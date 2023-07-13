using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class TotemManager : ManagerBase
    {
        public static TotemManager Instance { get; private set; }
        RunObject _activeRunObject { get; set; }
        RuntimeTotem _activeRuntimeTotem { get; set; }
        Dictionary<TotemObject, RuntimeTotem> _runtimeTotems = new Dictionary<TotemObject, RuntimeTotem>();
        bool _runtimeTotemsGenerated = false;
        int _activeTotemObjectIndex = 0;


        public int remainingFragments => _activeRuntimeTotem.remainingFragmentsAmount;
        public int collectedFragments
        {
            get
            {
                if (_activeRuntimeTotem == null)
                    return 0;
                return _activeRuntimeTotem.collectedFragmentsAmount;
            }
        }
        /// <summary> The amount of fragments required to assemble the totem. </summary>
        public int requiredFragmentsAmount => _activeRuntimeTotem.requiredFragments;
        public RuntimeTotem activeRuntimeTotem => _activeRuntimeTotem;
        public TotemObject activeTotemObject => _activeRunObject.totems[_activeTotemObjectIndex];


        #region Initialization
        protected override void MonoAwake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        public override void Init()
        {
            Debugger.LogInit("Init in Totem Manager.");
            Managers.totemManager = this;
        }
        public override void MyAwake()
        {
            Debugger.LogInit("MyAwake in Totem Manager.");
            Managers.runManager.OnRunStart += _OnRunStarted;
            Managers.runManager.OnPreNextLevelLoad += _OnPreNextLevelLoad;
        }

        public override bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            existingManager = FindObjectOfType<TotemManager>();

            if (existingManager == null)
                return false;

            if (existingManager.GetType() != this.GetType())
                Debug.LogError("Check manager name ! Be careful when copy-pasting ;)" +
                    " A " + existingManager.GetType().ToString() +
                    " is not a " + this.GetType().ToString());

            return existingManager != null;
        }
        #endregion


        /// <summary> Whether the player has collected and deposited all the framents in all the levels. </summary>
        public bool AllFragmentsDeposited()
        {
            bool toReturn = true;
            foreach (RuntimeTotem item in _runtimeTotems.Values)
            {
                if (item.remainingFragmentsAmount > 0)
                    toReturn = false;
            }
            return toReturn;
        }


        #region Important manager events
        public override void OnNewLevelLoaded()
        {
            //Debug.Log("Totem manager is doing somehting when new level is loaded");
        }

        private void _OnRunStarted(RunObject activeRunObject)
        {
            _activeRunObject = activeRunObject;
            _GenerateRuntimeTotems(activeRunObject);
            _LoadActiveRuntimeTotemAtIndex(0);
        }

        private void _OnPreNextLevelLoad(SceneLoadPair nextScenePair)
        {
            //Load the next runtime totem in the list of totems in the RunObject
            _LoadActiveRuntimeTotemAtIndex(_activeTotemObjectIndex + 1);
        }
        #endregion Important manager events


        private void _GenerateRuntimeTotems(RunObject activeRunObject)
        {
            Debug.Log("Generating runtime totems");
            //Convert the totems from the run into runtime totems for the levels to 
            foreach (TotemObject item in activeRunObject.totems)
            {
                if (_runtimeTotems.ContainsKey(item))
                {
                    Debug.Log("Already contains the key " + item.totemName);
                    continue;
                }

                _runtimeTotems.Add(item, new RuntimeTotem(item));
            }
            _runtimeTotemsGenerated = true;
        }

        private void _LoadActiveRuntimeTotemAtIndex(int index)
        {
            if (_activeRunObject.totems.Count == 0)
            {
                Debug.LogWarning($"No totems assigned in the {typeof(RunObject)}, named \"{_activeRunObject.name}\"");
                return;
            }

            //If there is no totem in the runtime totem for the current level, return the firts one
            if (index >= _activeRunObject.totems.Count)
            {
                _activeRuntimeTotem = _runtimeTotems[_activeRunObject.totems[0]];
                return;
            }

            _activeTotemObjectIndex = index;
            _activeRuntimeTotem = _runtimeTotems[_activeRunObject.totems[_activeTotemObjectIndex]];
        }

        public void ClearRuntimeTotem()
        {
            _activeRuntimeTotem = null;
            _activeTotemObjectIndex = 0;
        }

        public void DepositCollectedTotemFragments(List<TotemFragmentInfo> collectedFragments)
        {
            foreach (var item in collectedFragments)
            {
                _activeRuntimeTotem.AddNewCollectedFragment(item);
            }
            
            if (_activeRuntimeTotem.remainingFragmentsAmount == 0)
                Managers.eventManager.FireRoomEvent(RoomEvent.AllFragmentsCollected, this);


            /*_activeFragments.Remove(collectedTotem);
            _collectedFragmentsAmount += 1;
            Managers.eventManager.FireRoomEvent(RoomEvent.FragmentCollected, this);

            if (_activeFragments.Count == 0)
            {
                Managers.eventManager.FireRoomEvent(RoomEvent.AllFragmentsCollected, this);
            }*/
        }

        public RuntimeTotem GetRuntimeTotemFromReference(TotemObject targetTotem)
        {
            if (_runtimeTotemsGenerated == false) return null;

            RuntimeTotem toReturn = _activeRuntimeTotem;

            if (_runtimeTotems.ContainsKey(targetTotem))
                toReturn = _runtimeTotems[targetTotem];
            else
                Debugger.LogError($"The scriptable object \"{targetTotem.name}\" does not exist in the {typeof(RunObject)} named \"{_activeRunObject.name}\". Make sure the totem SO is assigned to the list of totems in the {typeof(RunObject)}.");

            return toReturn;
        }
    }
}
