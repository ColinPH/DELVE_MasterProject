using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropellerCap
{
    public static class InspectorMessageExtensions
    {
        /// <summary> Converts the custom InspectorMessageType to Unity MessageType. InspectorMessageType are used because they do not require the UnityEditor namespace and are therefore build friendly. </summary>
        public static MessageType Convert(this InspectorMessageType messageType)
        {
            switch (messageType)
            {
                case InspectorMessageType.Info:
                    return MessageType.Info;
                case InspectorMessageType.Warning:
                    return MessageType.Warning;
                case InspectorMessageType.Error:
                    return MessageType.Error;
                default:
                    return MessageType.Info;
            }
        }
    }
}
