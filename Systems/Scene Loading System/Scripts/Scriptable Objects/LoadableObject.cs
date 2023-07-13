using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public abstract class LoadableObject : ScriptableObject
    {
        public virtual string LoadableName => "Default Loadable Name";
        public virtual object SceneIdentifier => null;
        public virtual SceneGroup SceneGroup => SceneGroup.Unassigned;

        #region All Scenes
        public virtual List<string> GetAllSceneNames()
        {
            Debug.LogError("This should be overwriten.");
            return null;
        }
        public virtual List<UnityEngine.Object> GetAllSceneAssets()
        {
            Debug.LogError("This should be overwriten.");
            return null;
        }
        #endregion All Scenes


        #region Main and Optional Scenes
        public virtual List<string> GetMainSceneNames()
        {
            Debug.LogError("This should be overwriten.");
            return new List<string>();
        }
        public virtual List<UnityEngine.Object> GetMainSceneAssets()
        {
            Debug.LogError("This should be overwriten.");
            return null;
        }

        public virtual List<string> GetOptionalSceneNames()
        {
            Debug.LogError("This should be overwriten.");
            return new List<string>();
        }
        public virtual List<UnityEngine.Object> GetOptionalSceneAssets()
        {
            Debug.LogError("This should be overwriten.");
            return null;
        }
        #endregion Main and Optional Scenes

        /// <summary> Used by the custom inspector to convert the Scene assets to a list of strings because Standalone builds can not use UnityEngine.Object. </summary>
        public virtual void ConvertSceneObjectsToNames()
        {
            Debug.LogError("This should be overwriten.");
        }

        public virtual bool LoadableObjectIsValid(out string wrongAssetName)
        {
            Debug.LogError("This should be overwriten.");
            wrongAssetName = "";
            return false;
        }

    }
}