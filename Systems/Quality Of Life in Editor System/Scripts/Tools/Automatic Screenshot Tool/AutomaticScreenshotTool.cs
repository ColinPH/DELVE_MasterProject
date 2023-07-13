using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    public class AutomaticScreenshotTool : EditorWindow
    {
        static int _shotIntervalSec = 300;
        static double _lastScreenshotTime;
        static bool _isTakingScreenshots = false;

        static string _screenshotFolderPath = "";
        static string _identifier = "Iteration X";
        static string _userName = "User";

        // This method will be called automatically when Unity starts up
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            //If the editor has been started in batch mode from the automated builder, do not take screenshots
            if (Application.isBatchMode)
                return;

            QOLSettings settings = Utils.FindQOLSettings();
            if (settings.automatedScreenshotSettings.startProcessOnStartup == false)
                return;

            _isTakingScreenshots = true;
            //Debug.Log("Started taking screen shots");
            _StartScreenshotProcess();
        }

        #region Menu item

        [MenuItem("Propeller Cap/Tools/Take auto Screenshots")]
        private static void ControlScreenshotProcess()
        {
            _isTakingScreenshots = !_isTakingScreenshots;
            if (_isTakingScreenshots)
            {
                Debug.Log("Started taking screen shots");
                _StartScreenshotProcess();
            }
            else
            {
                Debug.Log("Stopped taking screen shots");
                _StopScreenshotProcess();
            }
        }

        [MenuItem("Propeller Cap/Tools/Take auto Screenshots", true)]
        private static bool ToggleMenuItemValidate()
        {
            Menu.SetChecked("Propeller Cap/Tools/Take auto Screenshots", _isTakingScreenshots);
            return true;
        }

        #endregion Menu item

        private static void _StartScreenshotProcess()
        {
            QOLSettings settings = Utils.FindQOLSettings();
            _shotIntervalSec = settings.automatedScreenshotSettings.secondsBetweenShots;
            
            //Next line has been added to stop screenshots from being taken whenever the editor enters playmode
            _lastScreenshotTime = EditorApplication.timeSinceStartup;

            EditorApplication.update += TakeScreenshotsEveryInterval;
        }

        private static void _StopScreenshotProcess()
        {
            EditorApplication.update -= TakeScreenshotsEveryInterval;
        }

        static void TakeScreenshotsEveryInterval()
        {
            if (EditorApplication.timeSinceStartup > _lastScreenshotTime + _shotIntervalSec)
            {
                //Debug.Log(EditorApplication.timeSinceStartup);
                _lastScreenshotTime = EditorApplication.timeSinceStartup;
                TakeScreenshots();
            }
        }

        static void TakeScreenshots()
        {
            if (_UserIsActive() == false)
            {
                //Debug.Log("Not taking a screenshot because user is not active.");
                return;
            }
            Debug.Log("Take screenshot");

            _userName = System.Environment.UserName;

            QOLSettings settings = Utils.FindQOLSettings();

            //If no screenshot folder assigned, create one outside the shared plastic folder
            DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath);
            string nonSharedProjectFolder = dirInfo.Parent.Parent.Parent.FullName;
            string newScreenshotsFolderPath = Path.Combine(nonSharedProjectFolder, "Automated Screenshots");

            if (!Directory.Exists(newScreenshotsFolderPath))
                Directory.CreateDirectory(newScreenshotsFolderPath);

            _screenshotFolderPath = newScreenshotsFolderPath;
            _identifier = settings.automatedScreenshotSettings.identifier;

            //Check the amount of screenshots already taken and warn the user if above threshold
            _CheckScreenshotsAmount(_screenshotFolderPath, settings);

            //Capture the game view
            _CaptureGameView(_screenshotFolderPath, _identifier, _userName);

            //Capture the scene view
            _CaptureSceneView(_screenshotFolderPath, _identifier, _userName);
        }

        private static void _CheckScreenshotsAmount(string path, QOLSettings settings)
        {
            string[] files = Directory.GetFiles(path);
            int fileCount = files.Length;
            int warningAmount = settings.automatedScreenshotSettings.amountWarningThreshold;
            if (fileCount >= warningAmount)
                Debug.Log("More than " + warningAmount + " screenshots have been taken.");
        }

        static bool _UserIsActive()
        {
            int unityProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
            bool isUnityFocused = _IsProcessFocused(unityProcessId);
            return isUnityFocused;
        }

        private static void _CaptureSceneView(string screenshotFolderPath, string identifier, string _userName)
        {
            RenderTexture sceneViewRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);

            if (SceneView.lastActiveSceneView == null)
                return;

            Camera sceneViewCamera = SceneView.lastActiveSceneView.camera;

            if (sceneViewCamera == null)
                return;

            sceneViewCamera.targetTexture = sceneViewRenderTexture;
            sceneViewCamera.Render();
            RenderTexture.active = sceneViewRenderTexture;
            Texture2D sceneViewTexture = new Texture2D(Screen.width, Screen.height);
            sceneViewTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            sceneViewTexture.Apply();
            sceneViewCamera.targetTexture = null;

            string fileName = $"{identifier}_{_userName}_{DateTime.Now:yyyy MM dd HH-mm-ss}_EditorSceneView.png";
            string path = Path.Combine(screenshotFolderPath, fileName);
            byte[] bytes = sceneViewTexture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
        }

        static void _CaptureGameView(string screenshotPath, string identifier, string _userName)
        {
            int width = Screen.width;
            int height = Screen.height;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            Camera camera = Camera.main;
            
            if (camera == null)
                return;

            camera.targetTexture = renderTexture;
            camera.Render();
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null;

            byte[] bytes = texture.EncodeToPNG();
            string filename = $"{identifier}_{_userName}_{DateTime.Now:yyyy MM dd HH-mm-ss}_EditorGameView.png";
            string path = Path.Combine(screenshotPath, filename);
            File.WriteAllBytes(path, bytes);
        }

        #region Check if Unity has user focus

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static bool _IsProcessFocused(int processId)
        {
            IntPtr hWnd = GetForegroundWindow();
            if (hWnd == IntPtr.Zero)
            {
                return false;
            }

            uint focusedProcessId;
            GetWindowThreadProcessId(hWnd, out focusedProcessId);

            return focusedProcessId == processId;
        }

        #endregion
    }
}
