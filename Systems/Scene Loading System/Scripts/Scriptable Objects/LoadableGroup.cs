using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public abstract class LoadableGroup : ScriptableObject
    {
        public virtual string GroupName => "Default Loadable Group Name";
        public virtual List<string> GetAllSceneNames()
        {
            Debugger.LogError("This should be overwriten.");
            return null;
        }
        public virtual List<UnityEngine.Object> GetAllScenesAssets()
        {
            Debugger.LogError("This should be overwriten.");
            return null;
        }

        public virtual List<LoadableObject> GetLoadableObjects()
        {
            Debugger.LogError("This should be overwriten.");
            return null;
        }

        public virtual bool GroupIsValid(out string wrongAssetName)
        {
            Debugger.LogError("This should be overwriten.");
            wrongAssetName = "";
            return false;
        }
    }
}