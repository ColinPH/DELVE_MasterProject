using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class HubManager : LocalManagerAction
    {
        public override void InvokeLocalManagerAction(LocalManager localManager)
        {
            //TODO move this to the save manager
            
        }

        protected override void MyStart()
        {
            base.MyStart();
            Managers.totemManager.ClearRuntimeTotem();
        }

        public void SaveTutorialCompletion()
        {
            Saver.progression.tutorialCompleted = true;
        }
    }
}