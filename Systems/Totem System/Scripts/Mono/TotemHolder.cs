using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    /// <summary> Component that keeps track of the collected fragments. Clears on player death. Used when interacting with the totem pillars. </summary>
    public class TotemHolder : MonoBehaviour
    {
        [Header("/!\\ Runtime Info /!\\")]
        [SerializeField] List<TotemFragmentInfo> _collectedFragments = new List<TotemFragmentInfo>();

        /// <summary> The amount of fragments currently in the totem holder. </summary>
        public int collectedFragmentsAmount => _collectedFragments.Count;

        public void CollectNewTotemFragment(TotemFragmentInfo totemFragmentInfo)
        {
            if (_collectedFragments.Contains(totemFragmentInfo))
            {
                Debugger.LogError($"The totem fragment \"{totemFragmentInfo.totemName}\" is already in the TotemHolder");
                return;
            }

            Saver.progression.hasCollectedFragment = true;
            Managers.playerManager.OnPlayerCollectObject?.Invoke(false, true);
            _collectedFragments.Add(totemFragmentInfo);
        }

        public List<TotemFragmentInfo> GetAllCollectedTotems(bool clearTotems = true)
        {
            List<TotemFragmentInfo> toReturn = new List<TotemFragmentInfo>(_collectedFragments);
            if (clearTotems)
                ClearAllFragments();
            return toReturn;
        }

        public void ClearAllFragments()
        {
            _collectedFragments.Clear();
        }

    }
}