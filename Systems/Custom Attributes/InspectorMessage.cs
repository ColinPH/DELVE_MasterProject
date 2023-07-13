using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class InspectorMessage
    {
        public string text = "";
        public InspectorMessageType messageType = InspectorMessageType.Info;
        public string errorFixButtonText = "Fix";

        bool _hasFixAction = false;
        Action _errorFixAction;

        public InspectorMessage(InspectorMessageType messageType)
        {
            this.messageType = messageType;
        }

        public InspectorMessage(string text, InspectorMessageType messageType)
        {
            this.text = text;
            this.messageType = messageType;
        }

        public bool hasFixAction => _hasFixAction;
        public Action errorFixAction 
        { 
            get => _errorFixAction; 
            set 
            {
                _hasFixAction = true;
                _errorFixAction = value;
            } 
        }
    }
}