using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace PropellerCap.EditorUsability
{
    public class AssetScreenshotTool : EditorWindow
    {
        public enum PictureType
        {
            png,
            jpg,
            exr,
            tga
        }

        int _oldLayer = 0;
        int _screenshotLayer = 30;
        GameObject _cameraObject;
        Camera _renderCamera;
        Color _backgroundColor = new Color(135, 135, 135);
        string _savePath = "";
        string _directoryPath = "";
        string _pictureName = "";
        Vector2Int _resolution = new Vector2Int(1920, 1080);
        PictureType _pictureType = PictureType.png;

        [MenuItem("Propeller Cap/Tools/Asset Screenshot")]
        public static void ShowWindow()
        {
            GetWindow<AssetScreenshotTool>("AssetScreenshotTool");
        }

        void OnEnable()
        {
            Selection.selectionChanged += Repaint;
        }

        private void OnDestroy()
        {
            Selection.selectionChanged -= Repaint;
        }

        private void OnGUI()
        {
            GUILayout.Space(5);
            GUILayout.Label("Folder to save the screenshots in", EditorStyles.boldLabel);
            GUIContent content = new GUIContent("Screenshot folder path");
            _savePath = EditorGUILayout.TextField(content, _savePath);

            //If the indicated path is not valid we show an error
            bool pathIsValid = Directory.Exists(_savePath);
            if (pathIsValid == false && _savePath != "")
                EditorGUILayout.HelpBox("The indicated path is not valid. Make sure it links to a folder.", MessageType.Error);

            //Warning if the indicated path leads outside the unity project
            if (PathIsInUnityProject(_savePath) == false && pathIsValid)
                EditorGUILayout.HelpBox("The path leads outside the unity project.", MessageType.Warning);
            else
                EditorGUILayout.HelpBox("You can get the path of a folder by right-clicking an asset folder and selecting the \"Show in Explorer\" option. Then paste it in the field above.", MessageType.Info);

            GUILayout.Space(5);
            GUILayout.Label("Settings", EditorStyles.boldLabel);

            content = new GUIContent("Name of the file");
            _pictureName = EditorGUILayout.TextField(content, _pictureName);

            content = new GUIContent("Background Colour");
            _backgroundColor = EditorGUILayout.ColorField(content, _backgroundColor);

            content = new GUIContent("Picture type");
            _pictureType = (PictureType)EditorGUILayout.EnumPopup(content, _pictureType);

            content = new GUIContent("Resolution");
            _resolution = EditorGUILayout.Vector2IntField(content, _resolution);

            GameObject selectedObj = Selection.activeGameObject;

            if (selectedObj == null)
            {
                EditorGUILayout.HelpBox("Select a gameObject in the hierarcy to take a screenshot from.", MessageType.Warning);
                return;
            }

            if (selectedObj.scene.IsValid() == false)
            {
                //Here the selected object is in the project assets
                EditorGUILayout.HelpBox("Make sure to select an object in the hierarchy, not in the project window.", MessageType.Warning);
                return;
            }

            if (pathIsValid == false)
            {
                EditorGUILayout.HelpBox("Add a path to save the screenshot.", MessageType.Warning);
                return;
            }

            if (GUILayout.Button("Take screenshot", GUILayout.Height(25f)))
            {
                _TakeScreenShotOfObject(selectedObj);
            }
        }

        private bool PathIsInUnityProject(string path)
        {
            string[] words = path.Split('/');

            if (words[0] == "Assets")
                return true;

            return false;
        }

        private void _TakeScreenShotOfObject(GameObject selectedObj)
        {
            _oldLayer = selectedObj.layer;
            selectedObj.layer = _screenshotLayer;

            //Create a new camera
            _cameraObject = new GameObject();
            _cameraObject.name = "Temporary Camera Object";
            _renderCamera = _cameraObject.AddComponent<Camera>();
            _renderCamera.cullingMask = 1 << _screenshotLayer;
            _renderCamera.clearFlags = CameraClearFlags.SolidColor;
            _renderCamera.backgroundColor = _backgroundColor;

            //Set the camera to the scene camera
            _cameraObject.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
            _cameraObject.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;

            //Take an image from the camera
            RenderTexture rt = new RenderTexture(_resolution.x, _resolution.y, 24);
            _renderCamera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(_resolution.x, _resolution.y, TextureFormat.RGB24, false);
            _renderCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, _resolution.x, _resolution.y), 0, 0);
            _renderCamera.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(rt);
            byte[] bytes = new byte[0];

            switch (_pictureType)
            {
                case PictureType.png:
                    bytes = screenShot.EncodeToPNG();
                    break;
                case PictureType.jpg:
                    bytes = screenShot.EncodeToJPG();
                    break;
                case PictureType.exr:
                    bytes = screenShot.EncodeToEXR();
                    break;
                case PictureType.tga:
                    bytes = screenShot.EncodeToTGA();
                    break;
            }

            //Save the screenshot in the screenshot directory
            string filePath;
            if (PathIsInUnityProject(_savePath))
            {
                filePath = Application.dataPath.Replace("/Assets", "") + "/" + _savePath + "/" + ScreenShotName();
                _directoryPath = Application.dataPath.Replace("/Assets", "") + "/" + _savePath;
            }
            else
            {
                filePath = _savePath + "/" + ScreenShotName();
                _directoryPath = _savePath;
            }

            File.WriteAllBytes(filePath, bytes);

            AssetDatabase.Refresh();
            Repaint();

            //Reset the selected object
            selectedObj.layer = _oldLayer;
            DestroyImmediate(_cameraObject);
        }

        private string ScreenShotName()
        {
            string type = ".png";

            switch (_pictureType)
            {
                case PictureType.png:
                    type = "png";
                    break;
                case PictureType.jpg:
                    type = "jpg";
                    break;
                case PictureType.exr:
                    type = "exr";
                    break;
                case PictureType.tga:
                    type = "tga";
                    break;
            }

            int index = 0;
            int iterations = 0;
            string testName = "";
            bool foundName = false;
            while (foundName == false || iterations < 1000)
            {
                testName = string.Format("{0} {1}.{2}", _pictureName, index, type);

                if (File.Exists(_directoryPath + "/" + testName))
                    index += 1;
                else
                    foundName = true;

                iterations += 1;
            }

            return testName;
        }
    }
}