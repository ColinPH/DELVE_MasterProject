using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class LevelUnloader : MonoBehaviour
    {
        public ActivationTarget _entryDoor;

        private bool _hasBeenActivated = false;

        private void Start()
        {
            if (_entryDoor == null)
                Debug.LogWarning("There is no door assigned to Level unloader " + gameObject.name);

            //var interactable = _entryDoor.GetComponent<IActivatable>();
            //interactable.RegisterToDeactivationComplete(_UnloadLevelAfterDoorClose);
        }

        /*public void UnloadPreviousLevel()
        {
            if (_hasBeenActivated) return;            
            _hasBeenActivated = true;

            Managers.runManager.UnloadPreviousLevel();
            //_entryDoor.Deactivate();
            //Wait for the door to close and unload the previous level
        }*/

        /// <summary> Called through a unity event in the hub when the player passes the entry door.</summary>
        public void UnloadPreviousLevel()
        {
            var interactable = _entryDoor.GetComponent<IActivatable>();
            interactable.RegisterToDeactivationComplete(_EntryDoorClosed);
            _entryDoor.Deactivate();            
        }

        private void _EntryDoorClosed()
        {
            StartCoroutine(Co_UnloadPreviousLevel());
        }

        public IEnumerator Co_UnloadPreviousLevel()
        {
            if (_hasBeenActivated) yield break;
            _hasBeenActivated = true;

            yield return StartCoroutine(Managers.runManager.Co_UnloadPreviousLevel());
        }
    }
}
