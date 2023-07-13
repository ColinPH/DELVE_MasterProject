using AK.Wwise;
using UnityEngine;

namespace PropellerCap
{
    public class SoundManager : ManagerBase
    {
        public static SoundManager Instance { get; private set; }

        [Header("Switches")]
        [SerializeField] AK.Wwise.Switch _sandStoneSwitch;
        [SerializeField] AK.Wwise.Switch _obscidianSwitch;
        [SerializeField] AK.Wwise.Switch _metalSwitch;
        [SerializeField] AK.Wwise.Switch _waterSwitch;
        [SerializeField] AK.Wwise.Switch _woodSwitch;
        [SerializeField] MaterialType _defaultMaterialType = MaterialType.SandStone;
        [Header("Debugs")]
        public bool debugSounds = false;
        public bool debugRTCPs = false;
        public bool debugSwitches = true;
        [Header("Warnings")]
        public bool showEmptyClipsWarnings = true;


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
            Debugger.LogInit("Init in sound manager");
            Managers.soundManager = this;
            Sound.soundManager = this;
        }
        public override void MyAwake()
        {
            Debugger.LogInit("MyAwake in sound manager");
        }
        public override void MyStart()
        {
            Debugger.LogInit("MyStart in sound manager");
        }
        public override bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            existingManager = FindObjectOfType<SoundManager>();

            if (existingManager == null)
                return false;

            if (existingManager.GetType() != this.GetType())
                Debug.LogError("Check manager name ! Be careful when copy-pasting ;)" +
                    " A " + existingManager.GetType().ToString() +
                    " is not a " + this.GetType().ToString());

            return existingManager != null;
        }
        #endregion


        #region Processing sound clips
        public void PlaySoundClip(SoundClip clip, GameObject targetObject)
        {
            if (clip == null)
            {
                if (showEmptyClipsWarnings)
                    Debug.LogWarning($"Sound clip is null or not assigned, object is : \"{targetObject.name}\".");
                return;
            }

            _CheckForAKGameObject(targetObject);

            if (clip.hasEvent)
                _PostWwiseEvent(clip.soundEvent, targetObject, clip.soundName);

            if (clip.hasTrigger)
                _PostWwiseTrigger(clip.soundTrigger, targetObject, clip.soundName);

            if (clip.hasState)
                _PostWwiseState(clip.soundState, targetObject, clip.soundName);
        }

        public void StopSoundClip(SoundClip clip, GameObject targetObject)
        {
            if (clip == null)
            {
                Debug.LogWarning("Sound clip is null, object is : " + targetObject.name);
                return;
            }

            if (clip.hasEvent)
                _StopWwiseEvent(clip.soundEvent, targetObject, clip.soundName);
        }
        #endregion Playing and Stopping sound clips


        #region Processing voice lines
        public void PlayVoiceLine(VoiceLine voiceLine, GameObject targetObject)
        {
            if (voiceLine == null)
            {
                if (showEmptyClipsWarnings)
                    Debug.LogWarning($"VoiceLineClip is null or not assigned, object is : \"{targetObject.name}\".");
                return;
            }

            _PostWwiseEvent(voiceLine.soundEvent, targetObject, $"voice line ID {voiceLine.LocalizationID} on SO \"{voiceLine.VoiceLineName}\"");
        }

        public void StopVoiceLine(VoiceLine voiceLine, GameObject targetObject)
        {
            if (voiceLine == null)
            {
                if (showEmptyClipsWarnings)
                    Debug.LogWarning($"VoiceLineClip is null or not assigned, object is : \"{targetObject.name}\".");
                return;
            }

            _StopWwiseEvent(voiceLine.soundEvent, targetObject, $"voice line ID {voiceLine.LocalizationID} on SO \"{voiceLine.VoiceLineName}\"");
        }
        #endregion Processing voice lines


        #region Posting & Stopping Events, Triggers, States to Wwise
        private void _PostWwiseEvent(AK.Wwise.Event soundEvent, GameObject targetObject, string soundName)
        {
            if (debugSounds)
                Debug.Log("Posting an EVENT to the AKSoundEngine, named : " + soundName);

            AkSoundEngine.PostEvent(soundEvent.Name, targetObject);
        }

        private void _StopWwiseEvent(AK.Wwise.Event soundEvent, GameObject targetObject, string soundName)
        {
            if (debugSounds)
                Debug.Log("Stopping an EVENT to the AKSoundEngine, named : " + soundName);

            soundEvent.Stop(targetObject);
        }

        private void _PostWwiseTrigger(AK.Wwise.Trigger soundEvent, GameObject targetObject, string soundName)
        {
            if (debugSounds)
                Debug.Log("Posting a TRIGGER to the AKSoundEngine, named : " + soundName);

            AkSoundEngine.PostTrigger(soundEvent.Name, targetObject);
        }

        private void _PostWwiseState(AK.Wwise.State soundEvent, GameObject targetObject, string soundName)
        {
            if (debugSounds)
                Debug.Log("Setting a STATE to the AKSoundEngine, named : " + soundName);

            AkSoundEngine.SetState(soundEvent.GroupId, soundEvent.Id);
        }
        #endregion Posting Events, Triggers, States to Wwise


        #region Sound RTPC
        public void SetRTPC(SoundClip clip, GameObject targetObject, float rtpcValue)
        {
            if (clip == null)
            {
                Debug.LogWarning("Sound clip is null, object is : " + targetObject.name);
                return;
            }

            _CheckForAKGameObject(targetObject);

            if (clip.hasRTCP)
            {
                if (debugRTCPs)
                    Debug.Log($"Posting an RTCP to the AKSoundEngine, event identifier \"{clip.soundName}\", " +
                        $"value: {rtpcValue}, ID: {clip.rtpc.Id}, " +
                        $"Name: {clip.rtpc.Name}");

                AkSoundEngine.SetRTPCValue(clip.rtpc.Name, rtpcValue, targetObject);
            }
        }
        #endregion Sound RTPC


        #region Changing switch state in Wwise
        /// <summary> Set the material switch in wwise based on the given MaterialType. </summary>
        public void SetSwitch(MaterialType matType, GameObject targetObject)
        {
            //Apply the default switch MaterialType if it has not been assigned
            if (matType == MaterialType.Unassigned)
            {
                Debugger.LogWarning($"{nameof(MaterialType)} not assigned on object \"{targetObject.name}\". Make sure to add a '{nameof(MaterialProperty)}' component to the object or main object.");
                matType = _defaultMaterialType;
            }
            
            AK.Wwise.Switch targetSwitch = _GetMaterialSwitch(matType);
            _SetSwitch(targetSwitch.GroupId, targetSwitch.Id, targetObject, matType.ToString());
        }

        private void _SetSwitch(uint switchGroupId, uint switchId, GameObject targetObject, string switchName)
        {
            if (debugSwitches)
                Debug.Log($"Setting {switchName} Wwise switch with target GameObject: \"{targetObject.name}\".");

            AkSoundEngine.SetSwitch(switchGroupId, switchId, targetObject);
        }

        private AK.Wwise.Switch _GetMaterialSwitch(MaterialType matType)
        {
            switch (matType)
            {
                case MaterialType.SandStone:
                    return _sandStoneSwitch;
                case MaterialType.Obscidian:
                    return _obscidianSwitch;
                case MaterialType.Metal:
                    return _metalSwitch;
                case MaterialType.Water:
                    return _waterSwitch;
                case MaterialType.Wood:
                    return _woodSwitch;
                default:
                    Debugger.LogError($"Missing switch statement case '{matType}' in '{nameof(SoundManager)}'.");
                    return null;
            }
        }
        #endregion Changing switch state in Wwise

        private static void _CheckForAKGameObject(GameObject targetObject)
        {
            //Add the AkGameObj component
            if (targetObject.GetComponent<AkGameObj>() == null)
                targetObject.AddComponent<AkGameObj>();

            //Register the AKGameObj
            AkSoundEngine.RegisterGameObj(targetObject);
        }
    }
}
