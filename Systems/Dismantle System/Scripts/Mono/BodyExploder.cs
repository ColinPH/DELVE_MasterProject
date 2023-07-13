using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class BodyExploder : WorldObject
    {
        enum ColliderType { MeshCollider = 0, BoxCollider = 1 }

        [Header("Explosion Settings")]
        [SerializeField] private float _explosionForce = 300f;
        [SerializeField] private float _explosionRadius = 5f;
        [SerializeField] private float _explosionUpwardsModifier = 4f;
        [SerializeField] private Vector3 _explosionOffset = new Vector3(0f, 0f, 0f);
        [Header("Explosion Settings")]
        [SerializeField] ColliderType _colliderType = ColliderType.MeshCollider;
        [ShowIf(nameof(_colliderType), 0)]
        [SerializeField] bool _convexMeshColliders = true;
        [Header("Sound On Pieces Collisions")]
        [SerializeField] bool _soundOnPieceCollisions = false;
        [SerializeField, ShowIf(nameof(_soundOnPieceCollisions))] SoundClip _collisionSound;
        [SerializeField, ShowIf(nameof(_soundOnPieceCollisions))] int _maxAmountCollisions = 5;
        [SerializeField, ShowIf(nameof(_soundOnPieceCollisions))] AnimationCurve _soundFiringChance;
        [SerializeField, ShowIf(nameof(_soundOnPieceCollisions))] float _curveTime = 2f;
        [SerializeField, ShowIf(nameof(_soundOnPieceCollisions))] LayerMask _contactMask;
        [Header("Objects destruction")]
        [SerializeField] bool _destroyMainObject = true;
        [SerializeField] bool _destroyPiecesAfterLifetime = true;
        [ShowIf(nameof(_destroyPiecesAfterLifetime))]
        [SerializeField] private float _piecesLifeTimeInSec = 4f;
        [SerializeField] List<GameObject> _piecesToDismantle = new List<GameObject>();



        #region WorldObject overrides
        public override string worldName => nameof(BodyExploder);
        #endregion WorldObject overrides


        #region Body explosion
        public void ExplodeDismantleBody()
        {
            foreach (GameObject obj in _piecesToDismantle)
            {
                obj.transform.SetParent(null);
                _AddCollider(obj);

                //Add the death timer for the pieces if they should be destroyed
                if (_destroyPiecesAfterLifetime)
                {
                    var deathTimer = obj.AddComponent<KillObjectAfterSeconds>();
                    deathTimer._timeBeforeDeath = _piecesLifeTimeInSec;
                }
            }

            StartCoroutine(_Co_AddRigidBody());
        }

        private IEnumerator _Co_AddRigidBody()
        {
            yield return null;

            foreach (GameObject obj in _piecesToDismantle)
            {
                //Add a rigidbody
                var rigidbody = obj.AddComponent<Rigidbody>();
                rigidbody.AddExplosionForce(_explosionForce, transform.position + transform.InverseTransformDirection(_explosionOffset), _explosionRadius, _explosionUpwardsModifier);
            
                //Add the collision sounds
                if (_soundOnPieceCollisions)
                {
                    var collisionSound = obj.AddComponent<SoundOnCollision>();
                    collisionSound.InitializeCollisionSound(_collisionSound, _maxAmountCollisions, _soundFiringChance, _curveTime, _contactMask);
                }
            }
            if (_destroyMainObject)
                Destroy(gameObject);
        }

        private Collider _AddCollider(GameObject targetObj)
        {
            switch (_colliderType)
            {
                case ColliderType.MeshCollider:
                    var meshColl = targetObj.AddComponent<MeshCollider>();
                    meshColl.convex = _convexMeshColliders;
                    return meshColl;
                case ColliderType.BoxCollider:
                    return targetObj.AddComponent<BoxCollider>();
                default:
                    Debugger.LogError($"{nameof(ColliderType)} not implemented in switch statement : {_colliderType}. Adding BoxCollider.");
                    return targetObj.AddComponent<BoxCollider>();
            }
        }
        #endregion Body explosion


        #region Custom Inspector
        public override List<InspectorMessage> GetInspectorWarnings()
        {
            List<InspectorMessage> toReturn = new List<InspectorMessage>();

            //Check that all the selected objects have a meshFilter
            bool containsNonMeshFilters = false;
            string wrongPiecesNames = "";
            List<GameObject> wrongItems = new List<GameObject>();
            foreach (GameObject item in _piecesToDismantle)
            {
                if (item.GetComponent<MeshFilter>() == null)
                {
                    containsNonMeshFilters = true;
                    wrongPiecesNames += "\n" + item.name;
                    wrongItems.Add(item);
                }
            }

            if (containsNonMeshFilters)
            {
                InspectorMessage message = new InspectorMessage(InspectorMessageType.Warning);
                message.text = $"One or more pieces to dismantle do not have a meshfilter component to create a collider." +
                    $" Make sure to select objects with a meshilter attached. \nFaulty objects are :{wrongPiecesNames}";
                message.errorFixButtonText = "Remove pieces from list";
                message.errorFixAction = () => 
                {
                    foreach (var item in wrongItems)
                    {
                        _piecesToDismantle.Remove(item);
                    }
                };
                toReturn.Add(message);
            }
            return toReturn;
        }
        #endregion Custom Inspector
    }
}