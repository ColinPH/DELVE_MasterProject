using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class CorridorUnloader : MonoBehaviour
    {
        public string _targetTag = "Player";
        public ActivationTarget targetDoor;

        private bool _hasBeenActivated = false;

        private void Start()
        {
            Collider coll = GetComponent<Collider>();
            if (coll == null)
            {
                Debug.LogWarning("There is no collider on object: " + gameObject.name);
            }

            if (coll.isTrigger == false)
                Debug.LogWarning("The collider is not a trigger on object: " + gameObject.name);

            if (targetDoor == null)
                Debug.LogWarning("There is no door assigned to Level unloader " + gameObject.name);

            //var interactable = targetDoor.GetComponent<IActivatable>();
            //interactable.RegisterToDeactivationComplete(_UnloadPreviousCorridor);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == _targetTag)
            {
                if (_hasBeenActivated)
                    return;
                _hasBeenActivated = true;

                if (targetDoor == null)
                {
                    UnloadPreviousCorridor();
                    return;
                }

                LocalLevelManager corridorManager = FindObjectOfType<LocalLevelManager>();
                if (corridorManager != null)
                    corridorManager.CloseEntryDoor();
            }
        }

        public void UnloadPreviousCorridor()
        {
            Managers.runManager.UnloadPreviousCorridor();
        }
    }
}