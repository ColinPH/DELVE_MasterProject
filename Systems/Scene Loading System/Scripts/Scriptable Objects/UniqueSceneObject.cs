using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Loadable/Unique Scene Object", order = 1)]
    public class UniqueSceneObject : LoadableObject
    {
        public string uniqueSceneName = "Default Unique Scene Name";
        public UniqueScene uniqueSceneType = UniqueScene.Unassigned;
        [SerializeField] List<UnityEngine.Object> _mainScenesAssetsToLoad = new List<UnityEngine.Object>();
        [SerializeField] List<UnityEngine.Object> _optionalScenesAssetsToLoad = new List<UnityEngine.Object>();
        [Header("Converted lists from Object to string:")]
        [SerializeField] List<string> _mainScenesToLoad = new List<string>();
        [SerializeField] List<string> _optionalScenesToLoad = new List<string>();

        #region LoadableObject overrides
        public override string LoadableName => uniqueSceneName;
        public override object SceneIdentifier => uniqueSceneType;
        public override SceneGroup SceneGroup => SceneGroup.UniqueScene;
        #region All Scenes
        public override List<string> GetAllSceneNames()
        {
            var toReturn = new List<string>(_mainScenesToLoad);
            toReturn.AddRange(_optionalScenesToLoad);
            return toReturn;
        }
        public override List<UnityEngine.Object> GetAllSceneAssets()
        {
            var toReturn = new List<UnityEngine.Object>(_mainScenesAssetsToLoad);
            toReturn.AddRange(_optionalScenesAssetsToLoad);
            return toReturn;
        }
        #endregion All Scenes

        #region Main and Optional Scenes
        public override List<string> GetMainSceneNames()
        {
            return _mainScenesToLoad;
        }
        public override List<UnityEngine.Object> GetMainSceneAssets()
        {
            return _mainScenesAssetsToLoad;
        }

        public override List<string> GetOptionalSceneNames()
        {
            return _optionalScenesToLoad;
        }
        public override List<UnityEngine.Object> GetOptionalSceneAssets()
        {
            return _optionalScenesAssetsToLoad;
        }
        #endregion Main and Optional Scenes


        public override void ConvertSceneObjectsToNames()
        {
            //Main scenes lists
            _mainScenesToLoad.Clear();
            foreach (var item in _mainScenesAssetsToLoad)
            {
                _mainScenesToLoad.Add(item.name);
            }

            //Optional scenes lists
            _optionalScenesToLoad.Clear();
            foreach (var item in _optionalScenesAssetsToLoad)
            {
                _optionalScenesToLoad.Add(item.name);
            }
        }


        public override bool LoadableObjectIsValid(out string wrongAssetName)
        {
#if UNITY_EDITOR
            foreach (UnityEngine.Object item in _mainScenesAssetsToLoad)
            {
                if (item is not UnityEditor.SceneAsset)
                {
                    wrongAssetName = item.name;
                    return false;
                }
            }
#endif
            wrongAssetName = "";
            return true;
        }
        #endregion
    
        public UniqueScene SceneType { get { return uniqueSceneType; } }
    }
}