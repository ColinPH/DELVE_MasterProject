using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;


namespace PropellerCap.QA
{
    public static class PlayerActionLogger
    {
        static string _defaultPlayerLogsFolderName = "Player Action Logs";
        static string _projectName = "PROJECTNAME";

        //Files and folder paths
        static string _mainLogsDirectory = "";
        static string _dailyLogsDirectory = "";
        static string _sessionLogsDirectory = "";
        static string _sessionLogsFilePath = "";
        static string _recordingLogsFilePath = "";

        static int _recordingIndex = 0;
        static bool _isRecording = false;

        static GameVersionData _versionData;

        static List<LogData> _logsToBeSaved;
        static bool _isWriting = false;
        static bool _hasBeenInitialized = false;

        //Controlling the access of the variables
        public static bool isRecording { get => _isRecording; }
        public static string logsDirectory { get => _sessionLogsDirectory; }
        public static bool hasBeenInitialized { get => _hasBeenInitialized; }
        


        public static void InitializeLogger()
        {
            if (_hasBeenInitialized) return;

            _versionData = new GameVersionData(Application.version);
            _projectName = Application.productName;
            _logsToBeSaved = new List<LogData>();
            _recordingIndex = 0;

            //Make sure that there is a folder to generate the logs data in

            //Create main logs folder
            LoggerUtility.CreateFolderAtPath(Application.persistentDataPath, _defaultPlayerLogsFolderName, out _mainLogsDirectory);

            //Create daily logs folder
            string folderName = "Player Logs_" + _GetDate() + "_" + _projectName;
            LoggerUtility.CreateFolderAtPath(_mainLogsDirectory, folderName, out _dailyLogsDirectory);

            //Create session logs folder
            folderName = "Play session_" + _GetTime();
            LoggerUtility.CreateFolderAtPath(_dailyLogsDirectory, folderName, out _sessionLogsDirectory);

            //Create session logs file
            _sessionLogsFilePath = _sessionLogsDirectory + LoggerUtility.GetPathSeparator() + "Main Session Logs.txt";
            if (File.Exists(_sessionLogsFilePath) == false) 
                File.Create(_sessionLogsFilePath).Close();

            _WriteFileHeader(_sessionLogsFilePath, false);

            _hasBeenInitialized = true;
            //Debug.Log(logsDirectory);
        }

        public static void SetGameVersionData(GameVersionData newVersionData)
        {
            _CheckInitialization();
            _versionData = newVersionData;
        }

        public static void OpenPlayerLogsFolderInWindowsExplorer()
        {
            _CheckInitialization();

            string pathToOpen = _mainLogsDirectory.Replace(@"/", @"\");   // explorer doesn't like front slashes
            System.Diagnostics.Process.Start("explorer.exe", "/select," + pathToOpen);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Public functions to add and control logs

        public static void AddLogs(string logsToAdd, PlayerLogsType logsType = PlayerLogsType.Unassigned)
        {
            _CheckInitialization();
            _logsToBeSaved.Add(new LogData(logsToAdd, logsType));

            _TryWriting();
        }

        public static void AddLogs(LogData logData)
        {
            _CheckInitialization();
            _logsToBeSaved.Add(logData);

            _TryWriting();
        }

        public static void AddLogs(List<LogData> logsData)
        {
            _CheckInitialization();
            foreach (LogData log in logsData)
            {
                _logsToBeSaved.Add(log);
            }

            _TryWriting();
        }

        public static void AddLogs(List<string> logsToAdd, PlayerLogsType logsType = PlayerLogsType.Unassigned)
        {
            _CheckInitialization();
            foreach (string log in logsToAdd)
            {
                _logsToBeSaved.Add(new LogData(log, logsType));
            }

            _TryWriting();
        }

        public static void AddLogs(Dictionary<string, PlayerLogsType> logsToAdd)
        {
            _CheckInitialization();
            foreach (KeyValuePair<string, PlayerLogsType> item in logsToAdd)
            {
                _logsToBeSaved.Add(new LogData(item.Key, item.Value));
            }

            _TryWriting();
        }

        public static void StartRecording()
        {
            _CheckInitialization();
            if (_isRecording) return;

            _isRecording = true;
            _recordingIndex += 1;
            _recordingLogsFilePath = _GetRecordingFileName();

            //Create the recodring logs file
            if (File.Exists(_recordingLogsFilePath) == false)
                File.Create(_recordingLogsFilePath).Close();

            _WriteFileHeader(_recordingLogsFilePath, true);
        }

        public static void StopRecording()
        {
            _CheckInitialization();
            if (_isRecording == false) return;
            
            _isRecording = false;
        }

        #endregion
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private static void _TryWriting()
        {
            if (_isWriting == false)
                _Task_WriteLogsToFile();
        }

        private static async void _Task_WriteLogsToFile()
        {
            await Task.Run(() => 
            { 
                List<LogData> tempLogs = new List<LogData>(_logsToBeSaved);
                _logsToBeSaved.Clear();

                _isWriting = true;

                foreach (LogData log in tempLogs)
                {
                    //Write to the main logs file
                    FileStream fs1 = new FileStream(_sessionLogsFilePath, FileMode.Append);
                    using (StreamWriter outputFile = new StreamWriter(fs1))
                    {
                        //outputFile.AutoFlush = true;
                        outputFile.WriteLineAsync(_ApplyTypeToLogText(log.logText, log.logType));
                    }
                    fs1.Close();
                
                    //Write to the recording logs file
                    if (_isRecording)
                    {
                        using (StreamWriter outputFile = new StreamWriter(_recordingLogsFilePath, true))
                        {
                            outputFile.WriteLineAsync(_ApplyTypeToLogText(log.logText, log.logType));
                        }
                    }
                }
                
                _isWriting = false;
            });

            if (_logsToBeSaved.Count > 0)
                _TryWriting();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Text and strings related stuff

        private static void _CheckInitialization()
        {
            if (_hasBeenInitialized == false)
                InitializeLogger();
        }

        private static void _WriteFileHeader(string filePath, bool isRecording)
        {
            using (StreamWriter outputFile = new StreamWriter(filePath, true))
            {
                outputFile.WriteLine("Play Session Date : " + _GetDate());

                if (isRecording)
                    outputFile.WriteLine("Recording Started at : " + _GetTime());
                else
                    outputFile.WriteLine("Play Session Started at : " + _GetTime());

                outputFile.WriteLine("Project Name : " + _projectName);
                outputFile.WriteLine("Game Version : " + (string)_versionData);
                outputFile.WriteLine("");
            }
        }

        private static string _ApplyTypeToLogText(string logText, PlayerLogsType logType)
        {
            string time = _GetTime(true);
            string prefix = "";
            string spaceText = "";

            if (logType != PlayerLogsType.Unassigned)
            {
                prefix = logType.ToString();
                spaceText = "_____";
            }

            return time + " " + spaceText + prefix + spaceText + logText;
        }

        private static string _GetRecordingFileName()
        {
            return _sessionLogsDirectory +
                LoggerUtility.GetPathSeparator() +
                "Recording Number " +
                _recordingIndex.ToString() + "_" +
                _GetTime() + ".txt";
        }

        private static string _GetDate()
        {
            return DateTime.Now.ToString("d/M/yyyy").Replace("/", "-");
        }

        private static string _GetTime(bool hasColumns = false)
        {
            if (hasColumns)
                return DateTime.Now.ToString("HH:mm:ss");
            else
                return DateTime.Now.ToString("HH:mm:ss").Replace(":", "-");
        }

        #endregion
    }
}