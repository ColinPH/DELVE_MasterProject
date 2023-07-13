using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Breakables/" + nameof(Dismantle))]
    public class Dismantle : ActivationTarget
    {
        [SerializeField] DismantleTarget _dismantleTarget = DismantleTarget.This;
        [SerializeField] DismantleType _dismantleType = DismantleType.Full;
        [ShowIf(nameof(_dismantleType), 0)]
        [SerializeField] GameObject _dismantledBodyPrefab;
        [ShowIf(nameof(_dismantleTarget), 1)]
        [SerializeField] GameObject _targetBody;

        private bool hasBeenActivated = false;

        #region Public accessors and events
        public DismantleTarget dismantleTarget => _dismantleTarget;
        public DismantleType dismantleType => _dismantleType;
        #endregion Public accessors and events

        #region MyBehaviour overrides
        public override string worldName => nameof(Dismantle);
        protected override void MyStart()
        {
            base.MyStart();

            switch (_dismantleType)
            {
                case DismantleType.Full:
                    m_PropertyIsNull<Dismantle>(_dismantledBodyPrefab == null, nameof(_dismantledBodyPrefab));
                    break;
                case DismantleType.Partial:
                    m_FetchForComponent<BodyExploder>();
                    break;
            }

            if (_targetBody == null)
                _targetBody = gameObject;
        }
        #endregion MyBehaviour overrides

        #region Dismantle methods
        /// <summary> Dismantle the targetObject. </summary>
        public override void Activate()
        {
            base.Activate();
            
            if (hasBeenActivated) return;

            _DismantleBody();
            InvokeActivationComplete();

            if (_dismantleType == DismantleType.Full)
                _targetBody.DestroyMyObject();
        }

        void _DismantleBody()
        {
            switch (_dismantleType)
            {
                case DismantleType.Full:
                    //Replace the entire prefab and explode it
                    GameObject dismantledObj = Instantiate(_dismantledBodyPrefab);

                    dismantledObj.transform.position = _targetBody.transform.position;
                    dismantledObj.transform.rotation = _targetBody.transform.rotation;
                    dismantledObj.transform.localScale = _targetBody.transform.localScale;

                    dismantledObj.GetComponent<BodyExploder>().ExplodeDismantleBody();
                    //The destruction of the current gameObject should be done after the ActivationCompleteInvoke
                    break;
                case DismantleType.Partial:
                    GetComponent<BodyExploder>().ExplodeDismantleBody();
                    break;
            }
            hasBeenActivated = true;
        }
        #endregion Dismantle methods



        #region Custom Inspector
        public override List<InspectorMessage> GetInspectorWarnings()
        {
            List<InspectorMessage> toReturn = new List<InspectorMessage>();

            //Check for a Body Exploder on the given prefab
            if (_dismantledBodyPrefab != null)
            {
                if (_dismantledBodyPrefab.GetComponent<BodyExploder>() == false)
                {
                    InspectorMessage message = new InspectorMessage(InspectorMessageType.Error);
                    message.text = $"The gameObject assigned in \"{_dismantledBodyPrefab.name}\" must " +
                    $"have a component of type {nameof(BodyExploder)}." +
                    $" Makesure it has one otherwise the dismanteling won't work.";
                    toReturn.Add(message);
                }
            }
            
            //Check that the current object has a body exploder if the target is set to This
            if (_dismantleTarget == DismantleTarget.This && _dismantleType == DismantleType.Partial && gameObject.GetComponent<BodyExploder>() == false)
            {
                InspectorMessage message = new InspectorMessage(InspectorMessageType.Error);
                message.text = $"Requires a component of type {nameof(BodyExploder)} for this object to be the dismantle target.";
                message.errorFixButtonText = "Add Now";
                message.errorFixAction = () => { gameObject.AddComponent<BodyExploder>(); };
                toReturn.Add(message);
            }

            return toReturn;
        }
        #endregion Custom Inspector
    }
}