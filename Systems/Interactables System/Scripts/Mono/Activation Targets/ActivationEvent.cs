using UnityEngine;
using UnityEngine.Events;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activation Targets/" + nameof(ActivationEvent))]
    public class ActivationEvent : WorldObject
    {
        [SerializeField] ActivationTarget _activationTarget;
        [SerializeField] UnityEvent _onActivationComplete;

        public override string worldName => nameof(ActivationEvent);
        protected override void MyAwake()
        {
            base.MyAwake();
            _activationTarget = m_FetchForComponent<ActivationTarget>();
            _activationTarget.RegisterToActivationComplete(_OnActivationComplete);
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
            _onActivationComplete?.Invoke();
        }
    }
}
