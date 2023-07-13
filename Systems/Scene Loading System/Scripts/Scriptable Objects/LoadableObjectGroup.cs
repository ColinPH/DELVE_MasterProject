using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace PropellerCap
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Loadable/Loadable Group", order = 1)]
    public class LoadableObjectGroup : LoadableGroup
    {
        public string loadableGroupName = "Loadable_Group_Name";
        [SerializeField] List<LoadableObject> _loadableObjects = new List<LoadableObject>();

        #region LoadableGroup overrides
        public override string GroupName => loadableGroupName;
        public override List<string> GetAllSceneNames()
        {
            List<string> toReturn = new List<string>();
            foreach (var item in _loadableObjects)
            {
                toReturn.AddRange(item.GetAllSceneNames());
            }
            return toReturn;
        }
        public override List<UnityEngine.Object> GetAllScenesAssets()
        {
            List<UnityEngine.Object> toReturn = new List<UnityEngine.Object>();
            foreach (var item in _loadableObjects)
            {
                toReturn.AddRange(item.GetAllSceneAssets());
            }
            return toReturn;
        }

        public override List<LoadableObject> GetLoadableObjects()
        {
            return _loadableObjects;
        }

        public override bool GroupIsValid(out string wrongAssetName)
        {
#if UNITY_EDITOR
            foreach (var item in _loadableObjects)
            {
                string name;
                if (item.LoadableObjectIsValid(out name) == false)
                {
                    wrongAssetName = name + " within " + GroupName;
                    return false;
                }
            }
#endif
            wrongAssetName = "";
            return true;
        }
        #endregion LoadableGroup overrides
    }
}