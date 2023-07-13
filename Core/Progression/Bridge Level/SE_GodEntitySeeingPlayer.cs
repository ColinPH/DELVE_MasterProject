using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class SE_GodEntitySeeingPlayer : WorldObject
    {
        [SerializeField] GameObject _targetObject;
        [SerializeField] float _cameraAngleLimit = 35f;
        [SerializeField] VoiceLineBase _godSeeingPlayerVoiceLine;
        [SerializeField] LayerMask _raycastMask;

        Transform _cameraTransform;
        bool _hasBeenActivated = false;

        #region WorldObject overrides
        public override string worldName => nameof(SE_GodEntitySeeingPlayer);

        protected override void MyStart()
        {
            base.MyStart();
            _cameraTransform = Camera.main.transform;
            _hasBeenActivated = Saver.progression.hasLookedInBridgeAbyss;
        }

        protected override void MyUpdate()
        {
            base.MyUpdate();

            if (_hasBeenActivated) return;

            //Check the angle of the camera with the upwards vector to know i fthe player is looking down
            Vector3 cameraAngle = _cameraTransform.forward;
            Vector3 downVector = Vector3.up * -1f;
            float angle = Mathf.Acos(Vector3.Dot(cameraAngle.normalized, downVector.normalized)) * Mathf.Rad2Deg; //Thanks ChatGPT
            if (angle <= _cameraAngleLimit)
            {
                _CastForVoiceLine();
            }

        }

        private void _CastForVoiceLine()
        {
            RaycastHit hit;
            if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out hit, 1000f, _raycastMask, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.gameObject == _targetObject)
                {
                    //Play the voice line
                    _hasBeenActivated = true;
                    Saver.progression.hasLookedInBridgeAbyss = true;
                    Sound.PlayVoiceLine(_godSeeingPlayerVoiceLine, gameObject);
                }
            }
        }

        #endregion WorldObject overrides
    }
}
