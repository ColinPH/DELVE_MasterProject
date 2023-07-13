using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PropellerCap
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Loadable/Level Object", order = 1)]
    public class LevelObject : LoadableObject
    {
        public string levelName = "Default Level Name";
        public Level levelType = Level.Unassigned;
        [SerializeField] List<UnityEngine.Object> _mainScenesAssetsToLoad = new List<UnityEngine.Object>();
        [SerializeField] List<UnityEngine.Object> _optionalScenesAssetsToLoad = new List<UnityEngine.Object>();
        [Header("Converted lists from Object to string:")]
        [SerializeField] List<string> _mainScenesToLoad = new List<string>();
        [SerializeField] List<string> _optionalScenesToLoad = new List<string>();

        public override string LoadableName => levelName;
        public override object SceneIdentifier => levelType;
        public override SceneGroup SceneGroup => SceneGroup.Level;

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
    }
}