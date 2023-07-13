using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class HUDContainersHandler : MonoBehaviour
    {
        [Header("Overlay Canvas")]
        [SerializeField] Transform _overlayBackgroundLayer;
        [SerializeField] Transform _overlayActiveLayer;
        [SerializeField] Transform _overlayForegroundLayer;

        [Header("Camera Space Canvas")]
        [SerializeField] Transform _cameraSpaceBackgroundLayer;
        [SerializeField] Transform _cameraSpaceActiveLayer;
        [SerializeField] Transform _cameraSpaceForegroundLayer;

        public Transform overlayBackgroundLayer => _overlayBackgroundLayer;
        public Transform overlayActiveLayer => _overlayActiveLayer;
        public Transform overlayForegroundLayer => _overlayForegroundLayer;
        public Transform cameraSpaceBackgroundLayer => _cameraSpaceBackgroundLayer;
        public Transform cameraSpaceActiveLayer => _cameraSpaceActiveLayer;
        public Transform cameraSpaceForegroundLayer => _cameraSpaceForegroundLayer;

        private void Start()
        {
            Managers.uiManager.hudContainersHandler = this;
        }

        public Transform GetContainer(HUDComponent hudComponent)
        {
            RenderContainerType containerType = hudComponent.containerType;
            switch (containerType)
            {
                case RenderContainerType.Unassigned:
                    Debugger.LogError($"{containerType} is not supported in {nameof(HUDContainersHandler)}, complete the switch statement. Or make sure the container type has been assigned in the {typeof(HUDComponent)} of the object \"{hudComponent.gameObject.name}\".");
                    return null;
                case RenderContainerType.overlayBackground:
                    return _overlayBackgroundLayer;
                case RenderContainerType.overlayActive:
                    return _overlayActiveLayer;
                case RenderContainerType.overlayForeground:
                    return _overlayForegroundLayer;
                case RenderContainerType.cameraSpaceBackground:
                    return _cameraSpaceBackgroundLayer;
                case RenderContainerType.cameraSpaceActive:
                    return _cameraSpaceActiveLayer;
                case RenderContainerType.cameraSpaceForeground:
                    return _cameraSpaceForegroundLayer;
                default:
                    Debugger.LogError($"{containerType} is not supported in {nameof(HUDContainersHandler)}, complete the switch statement.");
                    return null;
            }
        }
    }
}
