using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class TotemPillar : WorldObject
    {
        [Tooltip("If TRUE, the reference totem will used to display the collected matching fragments.")]
        [SerializeField] bool _useTotemReference = true;
        [SerializeField] TotemObject _referenceTotem;

        [SerializeField] bool _fetchSectionsInChildren = true;
        [SerializeField] List<TotemPillarSection> _activationSections = new List<TotemPillarSection>();
        [Header("/!\\ Runtime Information /!\\")]
        [SerializeField] RuntimeTotem _runtimeTotem;

        protected override void MyStart()
        {
            base.MyStart();

            m_PropertyIsNull<TotemPillar>((_referenceTotem == null), nameof(_referenceTotem));

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
            if (_useTotemReference)
                SetTargetRuntimeTotem(Managers.totemManager.GetRuntimeTotemFromReference(_referenceTotem));
            else
                SetTargetRuntimeTotem(Managers.totemManager.activeRuntimeTotem);
        }





        public void SetTargetRuntimeTotem(RuntimeTotem newTotemObject)
        {
            _runtimeTotem = newTotemObject;

            if (_runtimeTotem == null)
                return;

            Debug.Log($"Collected ({newTotemObject.collectedFragmentsAmount}) {newTotemObject.totemName}");

            //Check that the amount of activation sections matches the required amount of totem fragments
            if (_runtimeTotem.requiredFragments > _activationSections.Count)
            {
                Debugger.LogError($"Not enough activation sections ({_activationSections.Count}) for the amount of totem fragments ({newTotemObject.requiredFragments}). Totem name \"{newTotemObject.totemName}\" gameObject name \"{gameObject.name}\"");
            }

            for (int i = 0; i < newTotemObject.collectedFragmentsAmount; i++)
            {
                _activationSections[i].Activate();
                _activationSections[i].SpawnFragmentInSocket(_runtimeTotem.fragmentPrefab);
            }
        }
    }
}
