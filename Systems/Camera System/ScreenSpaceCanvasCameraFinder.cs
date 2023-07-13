using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class ScreenSpaceCanvasCameraFinder : MonoBehaviour
    {
        public Canvas targetCanvas;

        private void Start()
        {
            Managers.eventManager.SubscribeToMainGameEvent(GameEvent.SceneStart, _OnSceneStart);
        }

        private void _OnSceneStart(object eventCaller, SceneGroup sceneGroup, object sceneIdentifier)
        {
            targetCanvas.worldCamera = Camera.main;
            targetCanvas.planeDistance = 1f;
        }
    }
}
