using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace PropellerCap
{
    

    public class PlayerInputRecorder : MonoBehaviour
    {
        [SerializeField] bool _recordData = false;
        float _cameraSampleIntervalInSec = 0.04f;
        [SerializeField] Transform _playerTransform;
        [SerializeField] Transform _cameraTransform;

        List<InputListener> _simulatedInputList = new List<InputListener>();

        PlayerInputSamples _playerInputSamples = new PlayerInputSamples(
            new List<PlayerInputSample_Vector2>(),
            new List<PlayerInputSample_Button>(),
            new List<PlayerCursorSample>());

        PlayerInputController _inputController;
        bool _isRecording = false;
        float _elapsedTime = 0f;

        private void Awake()
        {
            _inputController = GetComponent<PlayerInputController>();
        }

        private void Start()
        {
            //Only record if option says so or if we are in a build
            if (_recordData/* || Application.isEditor == false*/)
                _isRecording = true;
            else
                return;

            //Register to the input actions
            foreach (InputType value in Enum.GetValues(typeof(InputType)))
            {
                ControlledInputAction action = _inputController.GetActionOfType(value);
                if (action != null)
                {
                    _simulatedInputList.Add(new InputListener(action, value, this));
                }
            }
        }

        private void Update()
        {
            if (_isRecording == false) return;

            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= _cameraSampleIntervalInSec)
            {
                _AddCameraRotationSample();
                _elapsedTime = 0f;
            }
        }

        private void OnDestroy()
        {
            if (_isRecording == false) return;
            _SaveRecordedData();
        }

        public void AddVector2Samples(InputType inputType, ActionEventType actionEventType, float castingTime, Vector2 sampleValue)
        {
            _playerInputSamples.vector2Samples.Add(new PlayerInputSample_Vector2(inputType, actionEventType, castingTime, sampleValue));
        }
        public void AddButtonSamples(InputType inputType, ActionEventType actionEventType, float castingTime)
        {
            _playerInputSamples.buttonSamples.Add(new PlayerInputSample_Button(inputType, actionEventType, castingTime));
        }

        private void _AddCameraRotationSample()
        {
            //Vector2 cameraRot = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 cameraRot = new Vector2(
                _cameraTransform.localEulerAngles.x,
                _playerTransform.eulerAngles.y);

            _playerInputSamples.cameraSamples.Add(new PlayerCursorSample(Time.time, cameraRot));
        }

        private void _SaveRecordedData()
        {
            Debug.Log($"Saving the data form the player input ({_playerInputSamples.vector2Samples.Count}) ({_playerInputSamples.buttonSamples.Count}) ({_playerInputSamples.cameraSamples.Count})");
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            string folderPath = Application.persistentDataPath + "/Recordings";
            //Create recordings folder if it doesn't exist
            if (Directory.Exists(folderPath) == false)
                Directory.CreateDirectory(folderPath);

            string fileName = $"PlayerInput_{Metrics.sessionData.ID}_{timeStamp}.bin";
            string finalPath = folderPath + "/" + fileName;

            using (FileStream fileStream = new FileStream(finalPath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, _playerInputSamples);
            }
        }

    }
}
