using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/" + nameof(Hookable))]
    public class Hookable : MonoBehaviour
    {
        [SerializeField] bool _isMovingObject = false;

        Vector3 _attachmentPoint = Vector3.zero;
        Vector3 _attachmentOffset = Vector3.zero;

        public delegate void HookableObjectDestructionHandler();
        public HookableObjectDestructionHandler OnHookableDestroyed { get; set; }

        private void OnDestroy()
        {
            OnHookableDestroyed?.Invoke();
        }

        public List<IInteractable> GetInteractables()
        {
            return GetComponents<IInteractable>().ToList();
        }

        public Vector3 GetHookAttachmentPoint()
        {
            return transform.position + transform.TransformDirection(_attachmentOffset);
        }

        public bool IsMovingObject()
        {
            return _isMovingObject;
        }

        /// <summary> When the hook has raycasted successfully and found this hookable component. </summary>
        public void HookRaycastShot(RaycastHit hitPoint)
        {
            _attachmentPoint = hitPoint.point;
            _attachmentOffset = transform.InverseTransformDirection(hitPoint.point - transform.position);
        }

        public void OnHookVisuallyAttached()
        {

        }
        public void OnHookDettached()
        {

        }
    }
}