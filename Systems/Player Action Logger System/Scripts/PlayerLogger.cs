using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Add a function to open a folder with the logs

namespace PropellerCap.QA
{
    public class PlayerLogger : MonoBehaviour
    {
        [Header("Special Keys to combine")]
        [SerializeField] Key specialKey1 = Key.LeftCtrl;
        [SerializeField] Key specialKey2 = Key.LeftShift;
        [Header("Keys to control the logger")]
        [SerializeField] Key openLogsFolderKey = Key.F;
        [SerializeField] Key startStopRecording = Key.R;
        [SerializeField] Key logQAAlert = Key.A;
        [Header("QA Alert log")]
        [SerializeField] string QAAlertLog = "This is a QA alert to mark a point in the logs.";

        private void Awake()
        {
            PlayerLogger[] loggers = GameObject.FindObjectsOfType<PlayerLogger>();

            if (loggers.Length > 1)
            {
                //If there is already a PlayerLogger in the scene we destroy this object
                Destroy(this.gameObject);
            }
        }

        private void Update()
        {
            if (Keyboard.current[specialKey1].isPressed &&
                Keyboard.current[specialKey2].isPressed)
            {
                //Open the windows explorer to access the logs
                if (Keyboard.current[openLogsFolderKey].wasPressedThisFrame)
                    PlayerActionLogger.OpenPlayerLogsFolderInWindowsExplorer();

                //Start and stop recording a specific set of logs
                if (Keyboard.current[startStopRecording].wasPressedThisFrame)
                {
                    if (PlayerActionLogger.isRecording)
                        PlayerActionLogger.StopRecording();
                    else
                        PlayerActionLogger.StartRecording();
                }

                //Log a QA Alert to help find something in the logs
                if (Keyboard.current[logQAAlert].wasPressedThisFrame)
                    PlayerActionLogger.AddLogs(QAAlertLog, PlayerLogsType.QA_Alert);
            }
        }
    }
}
