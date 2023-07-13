using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public static class Sound
    {
        static SoundManager _soundManager;
        public static SoundManager soundManager 
        { 
            get
            {
                if (_soundManager == null)
                    _soundManager = Managers.soundManager;
                return _soundManager; 
            }
            set
            {
                _soundManager = value;
            }
        }

        #region Sound clips
        public static void PlaySound(SoundClip clip, GameObject targetObject)
        {
            soundManager.PlaySoundClip(clip, targetObject);
        }
        public static void PlaySound(SoundClip clip, GameObject targetObject, MaterialType matType)
        {
            soundManager.SetSwitch(matType, targetObject);
            soundManager.PlaySoundClip(clip, targetObject);
        }

        public static void SetRTCP(SoundClip clip, GameObject targetObject, float rtcpValue)
        {
            soundManager.SetRTPC(clip, targetObject, rtcpValue);
        }

        public static void StopSound(SoundClip clip, GameObject targetObject)
        {
            soundManager.StopSoundClip(clip, targetObject);
        }
        #endregion Sound clips



        #region Voice lines
        public static void PlayVoiceLine(VoiceLineBase voiceLine, GameObject targetObject)
        {
            if (voiceLine == null)
            {
                Debug.LogWarning($"Voiceline is null or not assigned, object is : \"{targetObject.name}\".");
                return;
            }

            if (voiceLine is VoiceLine)
            {
                VoiceLine vLine = voiceLine as VoiceLine;
                soundManager.PlayVoiceLine(vLine, targetObject);
                HUD.subtitlesControler.DisplayText(vLine);
            }
            else if (voiceLine is VoiceLineGroup)
            {
                VoiceLine vLine = voiceLine as VoiceLineGroup;
                soundManager.PlayVoiceLine(vLine, targetObject);
                HUD.subtitlesControler.DisplayText(vLine);
            }
            else
            {
                Debugger.LogError($"Missing a cast for '{nameof(VoiceLineBase)}' as '{voiceLine.GetType()}'");
            }
        }

        public static void StopVoiceLine(VoiceLineBase voiceLine, GameObject targetObject)
        {
            if (voiceLine == null)
            {
                Debug.LogWarning($"Voiceline is null or not assigned, object is : \"{targetObject.name}\".");
                return;
            }

            if (voiceLine is VoiceLine)
            {
                VoiceLine vLine = voiceLine as VoiceLine;
                soundManager.StopVoiceLine(vLine, targetObject);
            }
            else if (voiceLine is VoiceLineGroup)
            {
                VoiceLineGroup vLine = voiceLine as VoiceLineGroup;
                //We must stop the last voiceLine played
                soundManager.StopVoiceLine(vLine.GetLastVoiceLine(), targetObject);
            }
            else
            {
                Debugger.LogError($"Missing a cast for '{nameof(VoiceLineBase)}' as '{voiceLine.GetType()}'");
            }
        }
        #endregion Voice lines
    }
}