using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    public enum ColliderType
    {
        None = 0,
        Normal = 1,
        Crouch = 2,
        Swing = 3
    }

    public class FirstPersonCharacterController : MonoBehaviour
    {
        [SerializeField] Transform targetCamera;
        [SerializeField] LayerMask groundedMask;
        [SerializeField] float _defaultDragValue = 0.3f;
        [Header("Movement calculation")]
        [Tooltip("Distance from the center at which the ground detection raycasts will take place.")]
        [SerializeField] float _castRadius = 0.5f;
        [Tooltip("Height at which the ground detection raycasts starts. Also serves as step height.")]
        [SerializeField] float _castHeightStart = 0.3f;
        [Tooltip("From the cast height start, how long is the ray to find the ground.")]
        [SerializeField] float _maxCastLength = 0.6f;
        [Header("Ground detection")]
        [Tooltip("Amount of casts around the player.")]
        [SerializeField] int _resolution = 8;
        [SerializeField] float _detectionRadius = 0.5f;
        [SerializeField] float _castDistance = 0.5f;
        [SerializeField] float _castVerticalOffset = 0.3f;

        Rigidbody _rigidbody;
        CapsuleCollider _collider;
        Plane _movementPlane;
        PlayerInputController _inputController;
        ControlledInputAction _movementAction;

        Vector3 _inputDirection = new Vector3();
        Vector3 _moveDirection = new Vector3();
        /// <summary> The object the player is currently standing on. </summary>
        GameObject _groundObject;

        [Header("/!\\ Debug info /!\\")]
        public float _playerSpeed = 0f;
        public Vector3 _playerVelocity = new Vector3();

        [SerializeField] bool _grounded = false;


        public bool grounded => _grounded;
        /// <summary> The object the player is currently standing on. </summary>
        public GameObject groundObject => _groundObject;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();
            _inputController= GetComponent<PlayerInputController>();
            _movementPlane = new Plane();
            _rigidbody.drag = _defaultDragValue;

            _movementAction = _inputController.GetActionOfType(InputType.Movement);
            _movementAction.performed += _UpdateInputDirection;
        }

        void Update()
        {
            _AlignInputDirectionToCamera();

            _grounded = 
                _ComputeGrounded();
            //Debug.Log("Velocity " + _rigidbody.velocity);
        }

        #region Input

        /// <summary>
        /// If the player is trying to move
        /// </summary>
        /// <returns></returns>
        public bool HasInputDirection()
        {
            return _inputDirection.sqrMagnitude > 0.0001f;
        }

        /// <summary>
        /// Linked to the input system, called whenever the movement input action is performed
        /// </summary>
        private void _UpdateInputDirection(ControlledInputContext context)
        {
            _inputDirection = new Vector3(context.GetVector2().x, 0f, context.GetVector2().y);
        }



        #endregion

        #region Velocity
        public Vector3 Velocity()
        {
            return _rigidbody.velocity;
        }

        public void SetVelocity(Vector3 newVelocity)
        {
            _rigidbody.velocity = newVelocity;
        }

        #endregion Velocity


        #region Drag

        public void SetDrag(float newDragValue)
        {
            _rigidbody.drag = newDragValue;
        }

        public void ResetDrag()
        {
            _rigidbody.drag = _defaultDragValue;
        }

        #endregion Drag


        #region Gravity

        public void SetGravityTo(bool newState)
        {
            _rigidbody.useGravity = newState;
        }

        #endregion Gravity


        #region Add forces

        public void CancelVerticalVelocity()
        {
            Vector3 velocity = _rigidbody.velocity;
            velocity.y = 0f;
            _rigidbody.velocity = velocity;
        }

        public void ApplyForce(Vector3 direction, float intensity, ForceMode forceMode = ForceMode.Force)
        {
            _rigidbody.AddForce(direction.normalized * intensity, forceMode);
        }

        public void ApplyForceAlongPlayerInput(float intensity, ForceMode forceMode = ForceMode.Force)
        {
            _rigidbody.AddForce(_moveDirection.normalized * intensity, forceMode);
        }

        #endregion Add forces


        #region Collider control

        public void SetColliderTypeTo(ColliderType colliderType)
        {
            //TODO expose collider variables in the inspector
            switch (colliderType)
            {
                case ColliderType.None:
                    _collider.enabled = false;
                    break;
                case ColliderType.Normal:
                    _collider.enabled = true;
                    _collider.center = new Vector3(0f, 0.9f, 0f);
                    _collider.height = 1.8f;
                    _collider.radius = 0.5f;
                    break;
                case ColliderType.Crouch:
                    _collider.enabled = true;
                    _collider.center = new Vector3(0f, 0.9f / 2f, 0f); //Center of the lower half
                    _collider.height = 1.8f / 2f;
                    _collider.radius = 0.2f;
                    break;
                case ColliderType.Swing:
                    _collider.enabled = true;
                    _collider.center = new Vector3(0f, 0.9f + 0.9f / 2f, 0f); //Center of the upper half
                    _collider.height = 1.8f / 2f;
                    _collider.radius = 0.2f;
                    break;
            }
        }

        #endregion Collider control


        #region Movement

        public void MoveAlongPlayerInputOnGround(float speed, float acceleration)
        {
            _ComputePlayerVelocity(speed, acceleration);

            _rigidbody.AddForce(_playerVelocity, ForceMode.VelocityChange);
            Vector3 oldVelocity = _rigidbody.velocity;
            //oldVelocity.y = 0f;
            //This apparently counters the momentum
            _rigidbody.AddForce(-oldVelocity, ForceMode.VelocityChange);
        }
        public void MoveAlongPlayerInputInAir(float speed, float acceleration)
        {
            _ComputePlayerVelocity(speed, acceleration);

            //Do not remove the following commented, it is used by the walking state to stop momentum
            //_rigidbody.AddForce(_moveDirection.normalized * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            _rigidbody.AddForce(_playerVelocity * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        /// <summary> Uses raycasting to determine the movement plane to move on based on the direction in which the player wants to move. </summary>
        private Vector3 _UpdateMovementPlane(Vector3 movementDireciton)
        {
            Vector3 center = transform.position;
            Vector3 centerVOffset = center;
            centerVOffset.y += _castHeightStart;
            Vector3 frontStart = centerVOffset + movementDireciton.normalized * _castRadius;
            Vector3 backStart = centerVOffset - movementDireciton.normalized * _castRadius;

            Debug.DrawLine(frontStart, frontStart - Vector3.up * _maxCastLength, Color.red);
            Debug.DrawLine(backStart, backStart - Vector3.up * _maxCastLength, Color.red);

            RaycastHit frontHit;
            if (Physics.Raycast(frontStart, -Vector3.up, out frontHit, _maxCastLength, groundedMask, QueryTriggerInteraction.Ignore))
            {
                
            }
            /*RaycastHit backHit;
            if (Physics.Raycast(backStart, -Vector3.up, out backHit, _maxCastLength, groundedMask, QueryTriggerInteraction.Ignore))
            {

            }*/

            return frontHit.point - transform.position;

            //_movementPlane.SetNormalAndPosition(Vector3.up, transform.position);
        }

        private void _AlignInputDirectionToCamera()
        {
            Vector3 alignedDirection = _inputDirection.x * targetCamera.right + _inputDirection.z * targetCamera.forward;
            alignedDirection.y = 0f;
            //_moveDirection = _UpdateMovementPlane(alignedDirection).normalized;


            Vector3 center = transform.position;
            Vector3 centerVOffset = center;
            centerVOffset.y += _castHeightStart;
            Vector3 frontStart = centerVOffset + alignedDirection.normalized * _castRadius;

            Debug.DrawLine(frontStart, frontStart - Vector3.up * _maxCastLength, Color.red);

            RaycastHit frontHit;
            Vector3 targetPoint;
            Color color = Color.white;
            if (Physics.Raycast(frontStart, -Vector3.up, out frontHit, _maxCastLength, groundedMask, QueryTriggerInteraction.Ignore))
            {
                //Made contact
                targetPoint = frontHit.point;
                color = Color.cyan;

                Vector3 midWayStart = centerVOffset + alignedDirection.normalized * _castRadius * 0.5f;
                RaycastHit midWayHit;
                //Also try at mid distance from the radius to have smoothtransitions on stairs ans such.
                if (Physics.Raycast(midWayStart, -Vector3.up, out midWayHit, _maxCastLength, groundedMask, QueryTriggerInteraction.Ignore))
                {
                    Vector3 midWayDirection = midWayHit.point - transform.position;
                    Vector3 frontDireciton = frontHit.point - transform.position;
                    //If slope is more important on midway point, use mid point instead
                    if (midWayDirection.y > frontDireciton.y)
                        targetPoint = midWayHit.point;
                }
            }
            else
            {
                //Could not make contact
                targetPoint = transform.position + alignedDirection.normalized * _castRadius; 
                color = Color.blue;
            }

            _moveDirection = targetPoint - transform.position;
            //_moveDirection = Vector3.ProjectOnPlane(alignedDirection, _movementPlane.normal).normalized;
            Debug.DrawLine(transform.position, transform.position + _moveDirection, color, 1f);
        }

        private void _ComputePlayerVelocity(float maxSpeed, float acceleration)
        {
            if (_inputDirection.sqrMagnitude > 0f) //Here the player is moving
            {
                //Increase player speed
                _playerSpeed += acceleration * Time.fixedDeltaTime;
                if (_playerSpeed > maxSpeed) _playerSpeed = maxSpeed;
            }
            else //Here the player doesn't move
            {
                _playerSpeed = 0f;
            }

            _playerVelocity = _moveDirection.normalized * _playerSpeed;
        }

        #endregion Movement

        #region Ground detection

        private bool _ComputeGrounded()
        {
            bool toReturn = false;
            List<Vector3> startPos = new List<Vector3>();

            if (_resolution < 4)
                Debugger.LogWarning("Ground detection resolution is lower than 4, it should be at least 4, for best result use 8.");

            float angle = 360f / _resolution;
            for (int i = 0; i < _resolution; i++)
            {
                Vector3 pos = transform.position;
                pos.x += Mathf.Cos(angle * i * Mathf.Deg2Rad) * _detectionRadius;
                //pos.y += verticalOffest;
                pos.z += Mathf.Sin(angle * i * Mathf.Deg2Rad) * _detectionRadius;
                startPos.Add(pos);
            }

            foreach (Vector3 item in startPos)
            {
                RaycastHit hit;
                Vector3 offsetedStart = item;
                offsetedStart.y += _castVerticalOffset;
                if (Physics.Raycast(offsetedStart, -Vector3.up, out hit, _castDistance, groundedMask, QueryTriggerInteraction.Ignore))
                {
                    toReturn = true;
                    _groundObject = hit.collider.gameObject;
                }
            }

            return toReturn;
        }

        #endregion Ground detection
    }
}
