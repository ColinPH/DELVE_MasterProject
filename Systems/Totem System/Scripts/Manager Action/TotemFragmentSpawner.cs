using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;

namespace PropellerCap
{
    public class TotemFragmentSpawner : LocalManagerAction
    {
        enum ExtinguishMoment { OnDeposit = 0, OnNextLevelLoad = 1, }
        [Header("Brazier extinguish")]
        [SerializeField] VoiceLineBase _brazierExtinguishVoiceLine;
        [SerializeField] ExtinguishMoment extinguishMethod = ExtinguishMoment.OnNextLevelLoad;
        [SerializeField] float _minWaitTime = 0.2f;
        [SerializeField] float _maxWaitTime = 0.5f;
        [Range(0f, 1f)]
        [SerializeField] float _turnOffAmount = 0.5f;
        [Header("Totem spawning")]
        [SerializeField] bool _spawnTotemFragments = true;
        [Tooltip("If TRUE, the reference totem will used to spawn the totem fragments.")]
        [SerializeField] bool _useTotemReference = false;
        [SerializeField] TotemObject referenceTotem;
        public UnityEvent onAllTotemsCollected;

        private bool _spawnedFragments = false;

        protected override void MyStart()
        {
            Managers.eventManager.SubscribeToRoomEvent(RoomEvent.AllFragmentsCollected, _OnAllTotemFragmentsCollected);
        }

        protected override void MonoDestroy()
        {
            base.MonoDestroy();
            Managers.eventManager.UnsubscribeFromRoomEvent(RoomEvent.AllFragmentsCollected, _OnAllTotemFragmentsCollected);
        }

        public override void InvokeLocalManagerAction(LocalManager localManager)
        {
            base.InvokeLocalManagerAction(localManager);
            
            if (_spawnTotemFragments)
            {
                SpawnTotemFragments();
            }

            if (extinguishMethod == ExtinguishMoment.OnNextLevelLoad)
            {
                int collectedAmount = Managers.totemManager.collectedFragments;
                int requiredAmount = Managers.totemManager.activeRuntimeTotem.requiredFragments;
                if (collectedAmount == requiredAmount)
                    _SwitchOffBraziers();
            }
        }

        private void _OnAllTotemFragmentsCollected()
        {
            if (extinguishMethod == ExtinguishMoment.OnDeposit)
                _SwitchOffBraziers();

            onAllTotemsCollected?.Invoke();
        }

        private void _SwitchOffBraziers()
        {
            Sound.PlayVoiceLine(_brazierExtinguishVoiceLine, gameObject);

            StartCoroutine(_Co_SwitchOffBraziers());
        }

        IEnumerator _Co_SwitchOffBraziers()
        {
            List<Brazier> braziers = (FindObjectsOfType<Brazier>()).ToList();
            List<Brazier> litBraziers = new List<Brazier>();
            foreach (var item in braziers)
            {
                if (item.IsIgnited())
                    litBraziers.Add(item);
            }

            int amountToExtinguish = (int)(litBraziers.Count * _turnOffAmount);
            for (int i = 0; i < amountToExtinguish; i++)
            {
                int randomIndex = Mathf.FloorToInt(Random.Range(0f, litBraziers.Count - 0.0001f));
                litBraziers[randomIndex].Extinguish();
                litBraziers.RemoveAt(randomIndex);
                float randomTime = Random.Range(_minWaitTime, _maxWaitTime);
                yield return new WaitForSecondsRealtime(randomTime);
            }
        }


        public void SpawnTotemFragments()
        {
            if (_spawnedFragments) 
                return;
            _spawnedFragments = true;

            Debug.Log("Spawn totem fragments");

            //Fetch the current totem
            RuntimeTotem totem;
            if (_useTotemReference)
                totem = Managers.totemManager.GetRuntimeTotemFromReference(referenceTotem);
            else
                totem = Managers.totemManager.activeRuntimeTotem;
            List<FragmentSpawnMarker> markers = FindObjectsOfType<FragmentSpawnMarker>().ToList();
            int amountSpawnMarkers = markers.Count;

            //Sort all remaining fragments for whether they already have an identifier or not
            List<TotemFragmentInfo> fragmentsWithID = new List<TotemFragmentInfo>();
            List<TotemFragmentInfo> fragmentsWithoutID = new List<TotemFragmentInfo>();
            foreach (TotemFragmentInfo item in totem.remainingFragments)
            {
                if (item.spawnIdentifier != "-1")
                    fragmentsWithID.Add(item);
                else
                    fragmentsWithoutID.Add(item);
            }

            //Spawn the fragments that already have a spawn identifier
            foreach (TotemFragmentInfo item in fragmentsWithID)
            {
                if (_FindMarkerWithID(item.spawnIdentifier, ref markers, out FragmentSpawnMarker matchingMarker))
                {
                    _SpawnFragment(item, matchingMarker);
                }
                else
                {
                    Debugger.LogError($"Could not find a {nameof(FragmentSpawnMarker)} with a spawn identifier that matches the totem fragment's spawn identifier. Has the {nameof(FragmentSpawnMarker)} been deleted from the scene ?");
                    item.ResetSpawnIdentifier();
                }
            }

            //Spawn the fragments that do not have a spawn identifier yet
            foreach (TotemFragmentInfo item in fragmentsWithoutID)
            {
                if (markers.Count == 0)
                {
                    Debugger.LogError($"There are not enough {nameof(FragmentSpawnMarker)} in the level. There are {amountSpawnMarkers} while you are trying to spawn {totem.remainingFragments.Count} fragments. Totem name is: {totem.totemName}");
                    break;
                }
                FragmentSpawnMarker spawnMarker = markers[0];
                item.SetSpawnIdentifier(spawnMarker.spawnIdentifier);
                _SpawnFragment(item, spawnMarker);
                markers.RemoveAt(0);
            }
        }

        private TotemFragment _SpawnFragment(TotemFragmentInfo item, FragmentSpawnMarker spawnMarker)
        {
            GameObject randomFrag = _GetRandomFragment();
            GameObject fragmentObj = Instantiate(randomFrag);
            fragmentObj.transform.position = spawnMarker.gameObject.transform.position;
            fragmentObj.transform.rotation = spawnMarker.gameObject.transform.rotation;
            fragmentObj.transform.SetParent(spawnMarker.gameObject.transform, true);

            TotemFragment fragment = fragmentObj.GetComponent<TotemFragment>();
            if (fragment == null)
                Debug.LogError("No script on the " + randomFrag.name + " which inherits from " + nameof(TotemFragment));

            fragment.InitializeFragment(item);
            return fragment;
        }

        private bool _FindMarkerWithID(string markerIdentifier, ref List<FragmentSpawnMarker> markers, out FragmentSpawnMarker matchingMarker)
        {
            bool toReturn = false;
            matchingMarker = null;
            int matchingIndex = -1;
            foreach (FragmentSpawnMarker marker in markers)
            {
                if (marker.spawnIdentifier == markerIdentifier)
                {
                    matchingIndex = markers.IndexOf(marker);
                    matchingMarker = marker;
                    toReturn = true;
                }
            }

            //Spawn markers identifiers that are already used by other fragments should be removed for when new fragments are spawned.
            //Two fragments should not have the same spawn identifier
            if (toReturn)
                markers.RemoveAt(matchingIndex);

            return toReturn;
        }

        private GameObject _GetRandomFragment()
        {
            if (_useTotemReference)
                return referenceTotem.fragmentPrefabs[Mathf.FloorToInt(Random.Range(0f, referenceTotem.fragmentPrefabs.Count - 0.001f))];
            else
                return Managers.totemManager.activeTotemObject.fragmentPrefabs[Mathf.FloorToInt(Random.Range(0f, Managers.totemManager.activeTotemObject.fragmentPrefabs.Count - 0.001f))];
        }
    }
}
