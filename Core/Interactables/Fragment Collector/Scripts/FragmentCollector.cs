using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    public class FragmentCollector : WorldObject, IInteractable
    {
        [SerializeField] VoiceLineBase _5FragmentsDepositVoiceLine;
        [SerializeField] bool _fetchSectionsInChildren = true;
        [SerializeField] List<TotemPillarSection> _activationSections = new List<TotemPillarSection>();
        //In the corridor this is used to start the process of unloading and loading
        [SerializeField] UnityEvent _onFragmentsDelivered;

        [SerializeField, Runtime(true)] RuntimeTotem _runtimeTotem;
        [SerializeField, Runtime] bool _fragmentsCollected = false;

        protected override void MyStart()
        {
            base.MyStart();

            //Fetch the sections to display the amount of fragments
            if (_fetchSectionsInChildren)
            {
                _activationSections = m_FetchForComponentsInChildren<TotemPillarSection>();
            }

            //Deactivate all the totem sections
            foreach (var item in _activationSections)
            {
                item.Deactivate();
            }

            //Fetch the runtime totem information to show the collected fragments
            SetTargetRuntimeTotem(Managers.totemManager.activeRuntimeTotem);
        }

        public void SetTargetRuntimeTotem(RuntimeTotem newTotemObject)
        {
            _runtimeTotem = newTotemObject;

            if (_runtimeTotem == null)
                return;

            Debug.Log($"Collected ({newTotemObject.collectedFragmentsAmount}) {newTotemObject.totemName}");
            
            _SpawnCollectedFragments(newTotemObject);
        }

        private void _SpawnCollectedFragments(RuntimeTotem runtimeTotem)
        {
            //Check that the amount of activation sections matches the required amount of totem fragments
            if (runtimeTotem.requiredFragments > _activationSections.Count)
            {
                Debugger.LogError($"Not enough activation sections ({_activationSections.Count}) for the amount of totem fragments ({runtimeTotem.requiredFragments}). {nameof(FragmentCollector)} on gameObject \"{gameObject.name}\".");
            }

            for (int i = 0; i < runtimeTotem.collectedFragmentsAmount; i++)
            {
                _activationSections[i].Activate();
                _activationSections[i].SpawnFragmentInSocket(runtimeTotem.fragmentPrefab);
            }
        }

        private void _CollectFragmentsFromHolder(GameObject callingEntity)
        {
            if (_fragmentsCollected) return;
            _fragmentsCollected = true;

            //Before depositing the fragments
            int oldRemainingFragments = Managers.totemManager.remainingFragments;
            int oldTotalFragments = Managers.totemManager.requiredFragmentsAmount;
            bool noTotemsDepositedInPriorRuns = (oldRemainingFragments == oldTotalFragments);

            TotemHolder holder = callingEntity.GetComponent<TotemHolder>();
            List<TotemFragmentInfo> collectedTotems = holder.GetAllCollectedTotems();
            
            Debugger.Log($"Player has deposited ({collectedTotems.Count}) fragments in the collector.");

            foreach (TotemFragmentInfo item in collectedTotems)
            {
                Metrics.runData.AddDepositedFragment(item.totemName, 1);
            }

            Managers.totemManager.DepositCollectedTotemFragments(collectedTotems);

            _SpawnCollectedFragments(_runtimeTotem);

            _onFragmentsDelivered?.Invoke();

            //Play voicelines            
            if (noTotemsDepositedInPriorRuns) //If it's the first time the player deposits the fragments in this level
                Sound.PlayVoiceLine(Managers.totemManager.activeRuntimeTotem.voiceLine_FirstFragmentInCollector, gameObject);
            else if (Managers.totemManager.remainingFragments == 0 && collectedTotems.Count > 0)
                Sound.PlayVoiceLine(_5FragmentsDepositVoiceLine, gameObject);

            //Destroy all temporary objets from other levels
            foreach (var item in FindObjectsOfType<DestroyOnFragmentDeposit>())
            {
                item.DestroyObject();
            }
        }

        #region IInteractable interface
        public string GetInteractionText()
        {
            return Managers.playerManager.PlayerInteractionText;
        }

        public void Highlight()
        {
            throw new NotImplementedException();
        }

        public void Interact(GameObject callingEntity)
        {
            _CollectFragmentsFromHolder(callingEntity);
        }

        public void InteractWithForceContinuous(Vector3 forceOrigin, Vector3 direction, float intensity)
        {
            throw new NotImplementedException();
        }

        public bool IsInteractable(GameObject initiatorObject)
        {
            if (_fragmentsCollected)
                return false;
            else
                return true;
        }

        public void OnInteractionStart(Action onInteractionCancelled)
        {
            throw new NotImplementedException();
        }

        public void OnInteractionStop()
        {
            throw new NotImplementedException();
        }

        public void OnInteractionWithForceStart(Vector3 forceOrigin, Vector3 direction, float intensity, GameObject pullingObject, object caller, Action onInteractionCancelled)
        {
            throw new NotImplementedException();
        }

        public void OnInteractionWithForceStop(Vector3 direction, float intensity)
        {
            throw new NotImplementedException();
        }
        #endregion IInteractable interface
    }
}
