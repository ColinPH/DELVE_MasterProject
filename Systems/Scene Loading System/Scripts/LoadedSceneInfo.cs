using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class LoadedSceneInfo
    {
        public List<string> loadedSceneNames;
        public SceneGroup sceneGroup;
        public LoadableObject loadable;
        /// <summary> TRUE if the scene is not a mandatory scene. FALSE if it is a mandatory scene. </summary>
        public bool disposable;

        public LoadedSceneInfo(LoadableObject loadable, SceneGroup sceneGroup, string sceneName)
        {
            this.loadedSceneNames = new List<string> { sceneName };
            this.sceneGroup = sceneGroup;
            this.loadable = loadable;
            disposable = (sceneGroup != SceneGroup.Mandatory);
        }

        public bool ContainsSceneNamed(string name)
        {
            return loadedSceneNames.Contains(name);
        }
        public void AddSceneNamed(string name)
        {
            if (loadedSceneNames.Contains(name))
            {
                Debug.LogError($"LoadedSceneInfo already containst the scene named{name}");
                return;
            }
            loadedSceneNames.Add(name);
        }
        public void RemoveSceneNamed(string name)
        {
            if (loadedSceneNames.Contains(name))
                loadedSceneNames.Remove(name);
            else
                Debug.LogError($"Trying to remove the scene name \"{name}\" from the loadedSceneInfo tied to the loadable \"{loadable.LoadableName}\". But the scene name has already been removed or has never been added.");
        }
        public bool HasLoadedScenes()
        {
            return loadedSceneNames.Count > 0;
        }
    }
}