using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace PropellerCap
{
    public class HookGun : MonoBehaviour
    {
        [SerializeField] GameObject _hookObj;
        [SerializeField] Transform _hookRestPoint;
        [SerializeField] Transform _gunTip;
        [SerializeField] Transform _ropeAnchorOnGun;
        [SerializeField] VisualEffectClip _hookDetachVFXClip;
        [SerializeField] LineRenderer _lineRenderer;

        bool _isExtended = false;

        private void Start()
        {
            _hookObj.transform.position = _hookRestPoint.position;
            _hookObj.transform.rotation = _hookRestPoint.rotation;

            if (_lineRenderer == null)
                _lineRenderer = _hookObj.GetComponent<LineRenderer>();

            //Hide the line
            _lineRenderer.enabled = false;
            //_lineRenderer.SetPosition(0, _ropeAnchorOnGun.position);
            //_lineRenderer.SetPosition(1, _hookObj.transform.position);
        }

        private void LateUpdate()
        {
            if (_isExtended)
            {
                _lineRenderer.SetPosition(0, _ropeAnchorOnGun.position);
                _lineRenderer.SetPosition(1, _hookObj.transform.position);
            }
        }

        public Transform GetGunTip()
        {
            return _gunTip;
        }

        public Transform DetachHook()
        {
            _hookObj.transform.SetParent(null);
            
            //Show the line
            _lineRenderer.enabled = true;
            _isExtended = true;

            VisualEffects.SpawnVFX(_hookDetachVFXClip, _gunTip);

            return _hookObj.transform;
        }

        public void AttachHook(Transform hookToAttach)
        {
            _isExtended = false;
            hookToAttach.position = _hookRestPoint.position;
            hookToAttach.rotation = _hookRestPoint.rotation;
            _hookObj = hookToAttach.gameObject;
            _hookObj.transform.SetParent(gameObject.transform); 

            //Reset the rope
            _lineRenderer.SetPosition(0, _ropeAnchorOnGun.position);
            _lineRenderer.SetPosition(1, _hookObj.transform.position);

            //Hide the line
            _lineRenderer.enabled = false;
        }
    }
}
