using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activation Targets/" + nameof(LogMessage))]
    public class LogMessage : WorldObject
    {
        [SerializeField] ActivationTarget _activationTarget;
        [SerializeField] InspectorMessageType _messageType = InspectorMessageType.Info;
        [SerializeField] string _activationText = "";
        [SerializeField] string _deactivationText = "";

        public override string worldName => nameof(LogMessage);
        protected override void MyAwake()
        {
            base.MyAwake();
            _activationTarget = m_FetchForComponent<ActivationTarget>();
            _activationTarget.RegisterToActivationComplete(_OnActivationComplete);
            _activationTarget.RegisterToDeactivationComplete(_OnDeactivationComplete);
        }

        public bool SetActivationTarget(ActivationTarget target)
        {
            if (_activationTarget == null)
            {
                _activationTarget = target;
                return true;
            }
            return false;
        }

        private void _OnActivationComplete()
        {
            string message = $"Activation message from object \"{gameObject.name}\" : {_activationText}";
            if (_activationText != "")
                _WriteLog(message, _messageType);
        }

        private void _OnDeactivationComplete()
        {
            string message = $"Deactivation message from object \"{gameObject.name}\" : {_deactivationText}";
            if (_deactivationText != "")
                _WriteLog(message, _messageType);
        }

        private void _WriteLog(string message, InspectorMessageType messageType)
        {
            switch (messageType)
            {
                case InspectorMessageType.Info:
                    Debug.Log(message);
                    break;
                case InspectorMessageType.Warning:
                    Debug.LogWarning(message);
                    break;
                case InspectorMessageType.Error:
                    Debug.LogError(message);
                    break;
            }
        }
    }
}
