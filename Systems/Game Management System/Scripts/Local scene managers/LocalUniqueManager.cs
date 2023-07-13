using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class LocalUniqueManager : LocalManager
    {
        [SerializeField] string _managerName = "Default_Unique_Manager";
        [SerializeField] List<LocalManagerAction> initializationActionList = new List<LocalManagerAction>();

        UniqueSceneLoadingInfo _loadingInfo;

        public override string LocalManagerName => _managerName;

        protected override void LocalManagerInitialization(object loadingInfo)
        {
            Debugger.LogInit("Local unique scene manager initialized.");

            //If the scene is already loaded in editor this will not receive loading settings
            if (loadingInfo == null)
                _loadingInfo = new UniqueSceneLoadingInfo();
            else
                _loadingInfo = (UniqueSceneLoadingInfo)loadingInfo;

            //Invoke all the manager actions in order
            foreach (var item in initializationActionList)
            {
                item.InvokeLocalManagerAction(this);
            }
        }
    }
}
