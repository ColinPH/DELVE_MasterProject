using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace PropellerCap
{
    public class SaveManager : ManagerBase
    {
        public static SaveManager Instance { get; private set; }

        [SerializeField] string _targetSaveFileName = "DelveSave";

        Dictionary<string, BaseSafeState> _objectsSafeStates = new Dictionary<string, BaseSafeState>();

        #region Initialization
        protected override void MonoAwake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        public override void Init()
        {
            Debugger.LogInit("Init in save manager");
            Managers.saveManager = this;
            Saver.manager = this;
            Saver.progression = new RuntimeProgression();
        }
        public override void MyAwake()
        {
            Debugger.LogInit("MyAwake in save manager");
        }
        public override void MyStart()
        {
            Debugger.LogInit("MyStart in save manager");
        }
        public override bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            existingManager = FindObjectOfType<SaveManager>();

            if (existingManager == null)
                return false;

            if (existingManager.GetType() != this.GetType())
                Debug.LogError("Check manager name ! Be careful when copy-pasting ;)" +
                    " A " + existingManager.GetType().ToString() +
                    " is not a " + this.GetType().ToString());

            return existingManager != null;
        }
        #endregion


        #region Save the data to disk
        public void SaveRuntimeProgression()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(_GetSaveFilePath(), FileMode.Create);
            formatter.Serialize(fileStream, Saver.progression);
            fileStream.Close();
        }

        public RuntimeProgression LoadRuntimeProgression()
        {
            if (!File.Exists(_GetSaveFilePath()))
            {
                Debug.LogWarning("Save file not found.");
                return null;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(_GetSaveFilePath(), FileMode.Open);
            RuntimeProgression data = formatter.Deserialize(fileStream) as RuntimeProgression;
            fileStream.Close();

            return data;
        }
        #endregion Save the data to disk


        #region Activated objects in scene
        public void RegisterObjectForActivation(string worldID, BaseSafeState activationState)
        {
            if (_objectsSafeStates.ContainsKey(worldID) == false)
                _objectsSafeStates.Add(worldID, activationState);
        }
        public T GetObjectSafeState<T>(string worldID) where T : BaseSafeState
        {
            if (_objectsSafeStates.ContainsKey(worldID))
            {
                return (T)_objectsSafeStates[worldID];
            }
            
            Debugger.LogError($"Object has not been registered for activation.");
            return default(T);
        }
        public void SetObjectSafeState(string worldID, BaseSafeState activationState)
        {
            if (_objectsSafeStates.ContainsKey(worldID))
            {
                _objectsSafeStates[worldID] = activationState;
            }
            else
                Debugger.LogError($"Object has not been registered for activation.");
        }
        public bool GetObjectSafeState(string worldID, out BaseSafeState safeState)
        {
            if (_objectsSafeStates.ContainsKey(worldID) == false)
            {
                Debugger.LogError($"Object has not been registered for activation.");
                safeState = null;
                return false;
            }

            safeState = _objectsSafeStates[worldID];
            return true;
        }
        #endregion Activated objects in scene


        private string _GetSaveFilePath()
        {
            return Path.Combine(_GetSavesFolder(), _targetSaveFileName + ".bin");
        }

        private string _GetSavesFolder()
        {
            string savesFolderPath = Application.persistentDataPath + "/Saves";

            if (Directory.Exists(savesFolderPath) == false)
                Directory.CreateDirectory(savesFolderPath);

            return savesFolderPath;
        }
    }
}
