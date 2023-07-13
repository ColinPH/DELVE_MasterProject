using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace PropellerCap
{
    public class GameLoop
    {
        /*bool _loopIsActive = false;
        bool _loopIsPaused = false;

        //New entities for MyBehaviour initialization
        List<WorldObject> _newWorldObjects = new List<WorldObject>();
        List<WorldObject> _newLocalManagers = new List<WorldObject>();
        List<WorldObject> _newManagers = new List<WorldObject>();

        //Existing entities for MyBehaviour updates
        List<WorldObject> _worldObjects = new List<WorldObject>();
        List<WorldObject> _localManagers = new List<WorldObject>();
        List<WorldObject> _managers = new List<WorldObject>();

        public GameLoop()
        {
            _loopIsActive = false;
            _loopIsPaused = false;
        }

        #region Loop control
        public void Start()
        {
            _loopIsActive = true;
        }
        public void Stop()
        {
            _loopIsActive = false;
        }

        public void Pause()
        {
            _loopIsPaused = true;
        }
        public void Resume()
        {
            _loopIsPaused = false;
        }
        #endregion Loop control


        #region MonoBehaviour calls
        public void LoopUpdate()
        {
            if (_loopIsActive)
            {
                if (_loopIsPaused == false)
                {
                    _InitializeNewWorldObjects();
                }

                _CallMyUpdateOn(_managers);

                if (_loopIsPaused == false)
                {
                    _CallMyUpdateOn(_localManagers);
                    _CallMyUpdateOn(_worldObjects);
                }
            }
        }

        public void LoopFixedUpdate()
        {
            if (_loopIsActive)
            {
                _CallMyFixedUpdateOn(_managers);

                if (_loopIsPaused == false)
                {
                    _CallMyFixedUpdateOn(_localManagers);
                    _CallMyFixedUpdateOn(_worldObjects);
                }
            }
        }

        public void LoopLateUpdate()
        {
            if (_loopIsActive)
            {
                _CallMyLateUpdateOn(_managers);

                if (_loopIsPaused == false)
                {
                    _CallMyLateUpdateOn(_localManagers);
                    _CallMyLateUpdateOn(_worldObjects);
                }
            }
        }
        #endregion MonoBehaviour calls


        #region Post scene loading process and WorldObject management
        /// <summary> Calls MyAwake and MyStart on the new WorldObjects, then clears the list. </summary>
        public void _InitializeNewWorldObjects()
        {
            //Call MyAwake on new WorldObjects
            _CallMyAwakeOn(_newManagers);
            _CallMyAwakeOn(_newLocalManagers);
            _CallMyAwakeOn(_newWorldObjects);

            //Copy the world objects in case new ones are adde in Start
            List<WorldObject> mWObjects = new List<WorldObject>(_newManagers);
            List<WorldObject> lmWObjects = new List<WorldObject>(_newLocalManagers);
            List<WorldObject> wObjects = new List<WorldObject>(_newWorldObjects);

            //Clear the lists
            _newManagers.Clear();
            _newLocalManagers.Clear();
            _newWorldObjects.Clear();

            //Call MyStart on new WorldObjects
            _CallMyStartOn(mWObjects);
            _CallMyStartOn(lmWObjects);
            _CallMyStartOn(wObjects);

            //Add new entities to the list
            _worldObjects.AddRange(_newWorldObjects);
            _localManagers.AddRange(_newLocalManagers);
            _managers.AddRange(_newManagers);
        }

        /// <summary> Calls PreLocalManagerInitialization on all new worldObjects. Then calls the LocalManagerInitialization. </summary>
        public void LocalManagersInitialization(object initializationSettings)
        {
            //Call pre local manager initialized on WorldObjects
            _CallPreLocalManagerInitializationOn(_newManagers);
            _CallPreLocalManagerInitializationOn(_newLocalManagers);
            _CallPreLocalManagerInitializationOn(_newWorldObjects);

            //Initialize new local managers
            _CallLocalManagerInitializationOn(_newLocalManagers, initializationSettings);
        }
        #endregion Post scene loading and WorldObject management


        #region Registrations and deregistrations

        #region WorldObjects
        public void RegisterNewWorldObject(WorldObject newWorldObject)
        {
            if (_newWorldObjects.Contains(newWorldObject))
            {
                Debug.LogError($"The WorldObject \"{newWorldObject.worldName}\" has already been registered to the GameLoop.");
                return;
            }
            _newWorldObjects.Add(newWorldObject);
        }

        public void DeregisterWorldObject(WorldObject worldObjectToRemove)
        {
            if (_worldObjects.Contains(worldObjectToRemove) == false)
            {
                if (_newWorldObjects.Contains(worldObjectToRemove))
                {
                    _newWorldObjects.Remove(worldObjectToRemove);
                    return;
                }
                Debug.LogError($"Trying to remove the WorldObject \"{worldObjectToRemove.worldName}\" from GameLoop but is not registered.");
                return;
            }
            Debugger.LogMyBehaviour($"Deregistering WorldObject \"{worldObjectToRemove.worldName}\"");
            _worldObjects.Remove(worldObjectToRemove);
        }
        #endregion WorldObjects

        #region Managers
        public void RegisterNewManager(ManagerBase newManager)
        {
            if (_newManagers.Contains(newManager))
            {
                Debug.LogError($"The Manager \"{newManager.worldName}\" has already been registered to the GameLoop.");
                return;
            }
            _newManagers.Add(newManager);
        }

        public void DeregisterManager(ManagerBase managerToRemove)
        {
            if (_managers.Contains(managerToRemove) == false)
            {
                if (_newManagers.Contains(managerToRemove))
                {
                    _newManagers.Remove(managerToRemove);
                    return;
                }
                Debug.LogError($"Trying to remove the Manager \"{managerToRemove.worldName}\" from GameLoop but is not registered.");
                return;
            }
            Debugger.LogMyBehaviour($"Deregistering Manager \"{managerToRemove.worldName}\"");
            _managers.Remove(managerToRemove);
        }
        #endregion Managers

        #region LocalManagers
        public void RegisterNewLocalManager(LocalManager newLocalManager)
        {
            if (_newLocalManagers.Contains(newLocalManager))
            {
                Debug.LogError($"The LocalManager \"{newLocalManager.LocalManagerName}\" has already been registered to the GameLoop.");
                return;
            }
            Debugger.LogMyBehaviour($"Registering new LocalManager \"{newLocalManager.LocalManagerName}\"");
            _newLocalManagers.Add(newLocalManager);
        }
        public void DeregisterLocalManager(LocalManager localManagerToRemove)
        {
            if (_localManagers.Contains(localManagerToRemove) == false)
            {
                if (_newLocalManagers.Contains(localManagerToRemove))
                {
                    _newLocalManagers.Remove(localManagerToRemove);
                    return;
                }
                Debug.LogError($"Trying to remove the LocalManager \"{localManagerToRemove.LocalManagerName}\" from GameLoop but is not registered.");
                return;
            }
            Debugger.LogMyBehaviour($"Deregistering LocalManager \"{localManagerToRemove.LocalManagerName}\"");
            _localManagers.Remove(localManagerToRemove);
        }
        #endregion LocalManagers

        #endregion Registrations and deregistrations


        #region WorldObjects calls for MyBehaviour
        private void _CallPreLocalManagerInitializationOn(List<WorldObject> targets)
        {
            foreach (WorldObject item in targets)
            {
                item.CallPreLocalManagerInitialization();
            }
        }

        private void _CallMyAwakeOn(List<WorldObject> targets)
        {
            foreach (WorldObject item in targets)
            {
                item.CallMyAwake();
            }
        }

        private void _CallMyStartOn(List<WorldObject> targets)
        {
            foreach (WorldObject item in targets)
            {
                item.CallMyStart();
            }
        }

        private void _CallMyUpdateOn(List<WorldObject> targets)
        {
            foreach (WorldObject item in targets)
            {
                item.CallMyUpdate();
            }
        }

        private void _CallMyFixedUpdateOn(List<WorldObject> targets)
        {
            foreach (WorldObject item in targets)
            {
                item.CallMyFixedUpdate();
            }
        }

        private void _CallMyLateUpdateOn(List<WorldObject> targets)
        {
            foreach (WorldObject item in targets)
            {
                item.CallMyLateUpdate();
            }
        }
        #endregion WorldObjects calls for MyBehaviour


        #region Manager calls
        



        #endregion Manager calls


        #region LocalManager calls
        private void _CallLocalManagerInitializationOn(List<WorldObject> targets, object initializationSettings)
        {
            foreach (LocalManager localManager in targets)
            {
                localManager.CallLocalManagerInitialization(initializationSettings);
            }
        }
        #endregion LocalManager calls
*/
    }
}