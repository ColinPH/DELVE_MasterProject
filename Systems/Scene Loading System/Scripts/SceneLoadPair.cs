using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class SceneLoadPair
    {
        public LoadableObject loadable;
        public object loadSettings;
        public object unloadSettings;
        public SceneTargets sceneTargets;

        public SceneLoadPair(LoadableObject loadable, object loadSettings, object unloadSettings, SceneTargets sceneTargets)
        {
            this.loadable = loadable;
            this.loadSettings = loadSettings;
            this.unloadSettings = unloadSettings;
            this.sceneTargets = sceneTargets;
        }
    }
}