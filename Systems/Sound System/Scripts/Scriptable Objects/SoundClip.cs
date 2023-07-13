using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Sound/Sound Clip", order = 1)]
    public class SoundClip : ScriptableObject
    {
        public string soundName = "Some cute name";
        [TextArea]
        public string description = "";
        public bool hasEvent = false;
        public AK.Wwise.Event soundEvent;
        public bool hasTrigger = false;
        public AK.Wwise.Trigger soundTrigger;
        public bool hasState = false;
        public AK.Wwise.State soundState;
        public bool hasRTCP = false;
        public AK.Wwise.RTPC rtpc;
    }
}