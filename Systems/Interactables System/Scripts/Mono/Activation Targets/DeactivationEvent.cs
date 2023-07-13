using UnityEngine;
using UnityEngine.Events;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activation Targets/" + nameof(DeactivationEvent))]
    public class DeactivationEvent : WorldObject
    {
        [SerializeField] ActivationTarget _activationTarget;
        [SerializeField] UnityEvent _onDeactivationComplete;

        public override string worldName => nameof(ActivationEvent);
        protected override void MyAwake()
        {
            base.MyAwake();
            _activationTarget = m_FetchForComponent<ActivationTarget>();
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

        private void _OnDeactivationComplete()
        {
            _onDeactivationComplete?.Invoke();
        }
    }
}