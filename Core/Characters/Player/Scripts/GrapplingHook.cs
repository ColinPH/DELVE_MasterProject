using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

namespace PropellerCap
{
    public class GrapplingHook : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        public Vector3 grapplePoint;
        public LayerMask whatIsGrappleable;
        public Transform gunTip, _camera;
        private float maxDistance = 100f;
        private SpringJoint joint;

        GrapplingState _gpHookState;
        PlayerInputController _inputController;
        ControlledInputAction _swingGrappleInput;

        float _grappleStartTime = 0f;
        bool _isAttached = false;
        public float holdTime = 0.2f;
        bool isHolding = false;

        void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;

            _inputController = GetComponent<PlayerInputController>();
        }

        private void Start()
        {

            _swingGrappleInput = _inputController.GetActionOfType(InputType.Secondary);
            _swingGrappleInput.started += _GrappleInputStart;
            _swingGrappleInput.canceled += _GrappleInputStop;
        }

        void Update()
        {
            

            /*if (Input.GetMouseButtonDown(0))
            {
                StartGrapple();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                StopGrapple();
            }*/
        }

        void LateUpdate()
        {
            DrawRope();
        }


        void _GrappleInputStart(ControlledInputContext context)
        {
            if (_isAttached)
            {
                StopGrapple();
                return;
            }

            Debug.Log("Grapple Start");
            _grappleStartTime = Time.realtimeSinceStartup;
            StartGrapple();
        }

        void _GrappleInputStop(ControlledInputContext context)
        {
            Debug.Log("Grapple Stop");

            if (Time.realtimeSinceStartup < _grappleStartTime + holdTime)
            {
                //Before holding
                _StartBatmanGrapple();
            }
            else
            {
                StopGrapple();
            }
        }

        private void _StartBatmanGrapple()
        {
            _isAttached = true;

            //remove joint because we will not use it
            Destroy(joint);

            RaycastHit hit;
            if (Physics.Raycast(_camera.position, _camera.forward, out hit, maxDistance, whatIsGrappleable))
            {
                grapplePoint = hit.point;

                _lineRenderer.positionCount = 2;
                currentGrapplePosition = gunTip.position;

                //Activate the grapling hook

                GetComponent<PlayerCharacter>().stateMachineBehaviour.TransitionToNewState<GrapplingState>(out _gpHookState);

                //_gpHookState.StartFastGrappling();
            }

        }

        /// <summary>
        /// Call whenever we want to start a grapple
        /// </summary>
        void StartGrapple()
        {
            _isAttached = true;
            GetComponent<PlayerCharacter>().stateMachineBehaviour.TransitionToNewState<GrapplingState>(out _gpHookState);



            RaycastHit hit;
            Debug.DrawRay(_camera.position, _camera.forward, Color.red, 2f);
            if (Physics.Raycast(_camera.position, _camera.forward, out hit, maxDistance, whatIsGrappleable))
            {
                grapplePoint = hit.point;
                joint = gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = grapplePoint;

                float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

                //The distance grapple will try to keep from grapple point. 
                joint.maxDistance = distanceFromPoint;// * 0.8f;
                joint.minDistance = distanceFromPoint * 0.25f;

                //Adjust these values to fit your game.
                joint.spring = 4.5f;
                joint.damper = 7f;
                joint.massScale = 4.5f;

                _lineRenderer.positionCount = 2;
                currentGrapplePosition = gunTip.position;
            }
        }


        /// <summary>
        /// Call whenever we want to stop a grapple
        /// </summary>
        public void StopGrapple()
        {
            _isAttached = false;
            _lineRenderer.positionCount = 0;

            //_gpHookState.StopGrappling();
            Destroy(joint);
        }


        private Vector3 currentGrapplePosition;

        void DrawRope()
        {
            //If not grappling, don't draw rope
            if (!joint) return;

            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

            _lineRenderer.SetPosition(0, gunTip.position);
            _lineRenderer.SetPosition(1, currentGrapplePosition);
        }
    }
}
