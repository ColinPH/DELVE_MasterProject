using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.ProBuilder;

namespace PropellerCap
{
    public class PlayerInputSamplesReader
    {
        private UnityEngine.Object inputRecordings;
        PlayerInputController _controller;
        Transform _playerTransform;
        Transform _cameraTransform;

        Dictionary<InputType, ControlledInputAction> _controlledInputActions = new Dictionary<InputType, ControlledInputAction>();

        PlayerInputSamples _playerInputSamples = new PlayerInputSamples(
            new List<PlayerInputSample_Vector2>(),
            new List<PlayerInputSample_Button>(),
            new List<PlayerCursorSample>());

        //Simulation Progression
        int _samplesIndex_Vector2 = 0;
        int _samplesIndex_Button = 0;
        int _currentCameraIndex = 0;
        int _totalCameraSamples = 0;
        float _cameraReferenceTime = 0f;

        public PlayerInputSamplesReader(UnityEngine.Object inputRecordings, PlayerInputController controller, Transform playerTransform, Transform cameraTransform)
        {
            _controller = controller;
            this.inputRecordings = inputRecordings;
            _playerTransform = playerTransform;
            _cameraTransform= cameraTransform;
            _GenerateControlledInputActions();
            _ReadSamplesData();
            _totalCameraSamples = _playerInputSamples.cameraSamples.Count;
            Debug.Log($"camera samples ({_totalCameraSamples})");
        }

        private void _GenerateControlledInputActions()
        {
            foreach (InputType value in Enum.GetValues(typeof(InputType)))
            {
                _controlledInputActions.Add(value, new ControlledInputAction(_controller, _controller.GetActionControlType(value)));
            }
        }

        public ControlledInputAction GetActionOfType(InputType inputType)
        {
            return _controlledInputActions[inputType];
        }

        /// <summary> Called in Update by input controller. Should trigger the Controlled input actions just like the PlayerInput class form Unity. </summary>
        public void ProcessSamples()
        {
            _ApplyCameraSamples(0.04f);
            _ApplyInputSamples_Vector2();
            _ApplyInputSamples_Button();
        }
        
        private void _ApplyCameraSamples(float sampleInterval)
        {
            //Do not go beyind the list of samples
            if (_currentCameraIndex + 1 >= _totalCameraSamples)
            {
                _controller.StopSimulation();
                Debug.Log($"Simulation Ended.");
                return;
            }

            if (Time.time >= _cameraReferenceTime)
            {
                var cameraSample = _playerInputSamples.cameraSamples[_currentCameraIndex];
                Vector2 currentPoint = cameraSample.position;
                //Calculate the next time to wait for
                var nextCameraSample = _playerInputSamples.cameraSamples[_currentCameraIndex + 1];
                _cameraReferenceTime = nextCameraSample.sampleTime;

                _playerTransform.eulerAngles = new Vector3(0f, currentPoint.y, 0f);
                _cameraTransform.localEulerAngles = new Vector3(currentPoint.x, 0f, 0f);

                _currentCameraIndex += 1;
            }


            

            /*// Get the current and next points
            Vector2 currentPoint = _playerInputSamples.cameraSamples[_currentCameraIndex].position;
            Vector2 nextPoint = _playerInputSamples.cameraSamples[_currentCameraIndex + 1].position;

            //If the rotation jumps by 360 because it is at the limit
            if (nextPoint.y < currentPoint.y && (Mathf.Abs(nextPoint.y - currentPoint.y) > 320f))
                nextPoint.y += 360f;
            if (nextPoint.y > currentPoint.y && (Mathf.Abs(nextPoint.y - currentPoint.y) > 320f))
                currentPoint.y += 360f;
            if (nextPoint.x < currentPoint.x && (Mathf.Abs(nextPoint.x - currentPoint.x) > 320f))
                nextPoint.x += 360f;
            if (nextPoint.x > currentPoint.x && (Mathf.Abs(nextPoint.x - currentPoint.x) > 320f))
                currentPoint.x += 360f;


            // Lerp the rotation around the X axis
            float playerRotation = (nextPoint.y - currentPoint.y) * Time.deltaTime * (1f / sampleInterval);
            float cameraRotation = (nextPoint.x - currentPoint.x) * Time.deltaTime * (1f / sampleInterval);
            
            // Apply the interpolated rotation
            _playerTransform.Rotate(0f, playerRotation, 0f);
            _cameraTransform.Rotate(cameraRotation, 0f, 0f, Space.Self);*/
        }

        private void _ApplyInputSamples_Vector2()
        {
            float referenceTime = Time.time;

            if (_playerInputSamples.vector2Samples.Count == 0) return;

            PlayerInputSample_Vector2 v2Sample = _playerInputSamples.vector2Samples[_samplesIndex_Vector2];
            //Check if the casting time of the next Vector2 input has been passed
            if (v2Sample.castingTime <= referenceTime)
            {
                //Simulate the input using the sample value
                _SimulateInput_Vector2(v2Sample);

                //Check the next inputs for whether they have the same input time
                int tempIndex = _samplesIndex_Vector2 + 1;
                bool castingTimeIsValid = true;
                while (castingTimeIsValid)
                {
                    //Make sure we don't go beyong the length of the array
                    if (tempIndex >= _playerInputSamples.vector2Samples.Count) break;

                    //Check if the next input sample casting time has been passed
                    PlayerInputSample_Vector2 nextV2Sample = _playerInputSamples.vector2Samples[tempIndex];
                    if (nextV2Sample.castingTime <= referenceTime)
                    {
                        _SimulateInput_Vector2(nextV2Sample);
                        _samplesIndex_Vector2 += 1; //Add 1 because it has bene activated
                    }
                    else
                    {
                        //If the casting time has not been passed, stop the loop
                        castingTimeIsValid = false;
                    }
                    tempIndex += 1;
                }
            }
        }

        private void _SimulateInput_Vector2(PlayerInputSample_Vector2 v2Sample)
        {
            if (v2Sample.actionEventType == ActionEventType.Started)
                _controlledInputActions[v2Sample.inputType].SimulateActionStarted(new ControlledInputContext(v2Sample.sampleValue));
            else if (v2Sample.actionEventType == ActionEventType.Performed)
                _controlledInputActions[v2Sample.inputType].SimulateActionPerformed(new ControlledInputContext(v2Sample.sampleValue));
            else if (v2Sample.actionEventType == ActionEventType.Canceled)
                _controlledInputActions[v2Sample.inputType].SimulateActionCanceled(new ControlledInputContext(v2Sample.sampleValue));
        }

        private void _ApplyInputSamples_Button()
        {
            float referenceTime = Time.time;

            if (_playerInputSamples.buttonSamples.Count == 0) return;

            PlayerInputSample_Button buttonSample = _playerInputSamples.buttonSamples[_samplesIndex_Button];
            //Check if the casting time of the next Button input has been passed
            if (buttonSample.castingTime <= referenceTime)
            {
                //Simulate the input using the sample value
                _SimulateInput_Button(buttonSample);

                //Check the next inputs for whether they have the same input time
                int tempIndex = _samplesIndex_Button + 1;
                bool castingTimeIsValid = true;
                while (castingTimeIsValid)
                {
                    //Make sure we don't go beyong the length of the array
                    if (tempIndex >= _playerInputSamples.buttonSamples.Count) break;

                    //Check if the next input sample casting time has been passed
                    PlayerInputSample_Button nextButtonSample = _playerInputSamples.buttonSamples[tempIndex];
                    if (nextButtonSample.castingTime <= referenceTime)
                    {
                        _SimulateInput_Button(nextButtonSample);
                        _samplesIndex_Button += 1; //Add 1 because it has bene activated
                    }
                    else
                    {
                        //If the casting time has not been passed, stop the loop
                        castingTimeIsValid = false;
                    }
                    tempIndex += 1;
                }
            }
        }
        private void _SimulateInput_Button(PlayerInputSample_Button buttonSample)
        {
            if (buttonSample.actionEventType == ActionEventType.Started)
                _controlledInputActions[buttonSample.inputType].SimulateActionStarted(new ControlledInputContext());
            else if (buttonSample.actionEventType == ActionEventType.Performed)
                _controlledInputActions[buttonSample.inputType].SimulateActionPerformed(new ControlledInputContext());
            else if (buttonSample.actionEventType == ActionEventType.Canceled)
                _controlledInputActions[buttonSample.inputType].SimulateActionCanceled(new ControlledInputContext());
        }

        private void _ReadSamplesData()
        {
#if UNITY_EDITOR
            string filePath = AssetDatabase.GetAssetPath(inputRecordings);
            using (FileStream stream = File.OpenRead(filePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                _playerInputSamples = (PlayerInputSamples)formatter.Deserialize(stream);
            }
#endif
        }
    }
}
