
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropellerCap
{
    public class WorldObject : MonoBehaviour
    {
        [SerializeField, HideInInspector] private string _worldID = "";

        public virtual string worldName => "Default world name";
        /// <summary> Whether the MyBehaviour methods should use try/catch when being invoked. Can have a slight impact on performances but will prevent errors from stopping the stack. </summary>
        [HideInInspector] public bool applyTryCatch = true;

        #region World identificator with GUID
        /// <summary> GUID identificator of the worldObject. Should be generated the first time the worldObject is created. </summary>
        public string worldID
        {
            get
            {
                if (HasGUID == false)
                    Debug.LogError($"GUID has not been generated. Object is \"{gameObject.name}\", component is '{worldName}'");
                return _worldID;
            }
        }
        /// <summary> Returns whether a GUID has been generated. </summary>
        public bool HasGUID => !string.IsNullOrEmpty(_worldID); //Returns the inverse

        /// <summary> Generates a GUID for the object if there isn't already one. Shows warning if there is already one. </summary>
        [ContextMenu("Generate GUID")]
        public void GenerateGUID()
        {
            if (string.IsNullOrEmpty(_worldID))
            {
                _worldID = Guid.NewGuid().ToString();
            }
            else
            {
                Debug.LogError($"Trying to set the GUID but there is already one. Object is \"{gameObject.name}\", component is '{worldName}'");
            }
        }

        /// <summary> Erases the GUID saved in the worldObject. Should not be used. Exists for inspector context menu. </summary>
        [ContextMenu("Erase GUID")]
        public void EraseGUID()
        {
            _worldID = "";
        }
        #endregion World identificator with GUID



        #region MonoBehaviour
        private void Awake()
        {
            Managers.gameManager.RegisterNewWorldObject(this);
            applyTryCatch = Managers.debuggerManager._applyTryCatchOnMyBehaviour;
            MonoAwake();
        }

        protected virtual void MonoAwake()
        {
            
        }

        private void OnDestroy()
        {
            Managers.gameManager.DeregisterWorldObject(this);
            MonoDestroy();
        }

        protected virtual void MonoDestroy()
        {
            
        }
        #endregion



        public void DestroyMyObject()
        {
            if (applyTryCatch)
            {
                try
                {
                    MyDestroy();
                    Destroy(gameObject);
                }
                catch (Exception ex)
                {
                    Debugger.LogError($"Error when calling MyDestroy on WorldObject \"{worldName}\" on GameObject \"{gameObject.name}\". Details are :\n" +
                        $"{ex.Message} \n {ex.StackTrace}");
                }
            }
            else
            {
                MyDestroy();
                Destroy(gameObject);
            }
        }

        #region MyBehaviour controls for the GameManager
        public void CallPreLocalManagerInitialization()
        {
            if (applyTryCatch)
            {
                try
                {
                    PreLocalManagerInitialization();
                }
                catch (Exception ex)
                {
                    Debugger.LogError($"Error when calling PreLocalManagerInitialization on WorldObject \"{worldName}\" on GameObject \"{gameObject.name}\". Details are :\n" +
                        $"{ex.Message} \n {ex.StackTrace}");
                }
            }
            else
            {
                PreLocalManagerInitialization();
            }
        }

        public void CallMyAwake()
        {
            if (applyTryCatch)
            {
                try
                {
                    MyAwake();
                }
                catch (Exception ex)
                {
                    Debugger.LogError($"Error when calling MyAwake on WorldObject \"{worldName}\" on GameObject \"{gameObject.name}\". Details are :\n" +
                        $"{ex.Message} \n {ex.StackTrace}");
                }
            }
            else
            {
                MyAwake();
            }
        }

        public void CallMyStart()
        {
            if (applyTryCatch)
            {
                try
                {
                    MyStart();
                }
                catch (Exception ex)
                {
                    Debugger.LogError($"Error when calling MyStart on WorldObject \"{worldName}\" on GameObject \"{gameObject.name}\". Details are :\n" +
                        $"{ex.Message} \n {ex.StackTrace}");
                }
            }
            else
            {
                MyStart();
            }
        }

        public void CallMyUpdate()
        {
            if (applyTryCatch)
            {
                try
                {
                    MyUpdate();
                }
                catch (Exception ex)
                {
                    Debugger.LogError($"Error when calling MyUpdate on WorldObject \"{worldName}\" on GameObject \"{gameObject.name}\". Details are :\n" +
                        $"{ex.Message} \n {ex.StackTrace}");
                }
            }
            else
            {
                MyUpdate();
            }
        }

        public void CallMyFixedUpdate()
        {
            if (applyTryCatch)
            {
                try
                {
                    MyFixedUpdate();
                }
                catch (Exception ex)
                {
                    Debugger.LogError($"Error when calling MyFixedUpdate on WorldObject \"{worldName}\" on GameObject \"{gameObject.name}\". Details are :\n" +
                        $"{ex.Message} \n {ex.StackTrace}");
                }
            }
            else
            {
                MyFixedUpdate();
            }
        }

        public void CallMyLateUpdate()
        {
            if (applyTryCatch)
            {
                try
                {
                    MyLateUpdate();
                }
                catch (Exception ex)
                {
                    Debugger.LogError($"Error when calling MyLateUpdate on WorldObject \"{worldName}\" on GameObject \"{gameObject.name}\". Details are :\n" +
                        $"{ex.Message} \n {ex.StackTrace}");
                }
            }
            else
            {
                MyLateUpdate();
            }
        }
        #endregion MyBehaviour controls for the GameManager



        #region Virtual methods for MyBehaviour
        protected virtual void PreLocalManagerInitialization()
        {

        }

        protected virtual void MyAwake()
        {

        }

        protected virtual void MyStart()
        {

        }

        protected virtual void MyUpdate()
        {

        }

        protected virtual void MyFixedUpdate()
        {

        }

        protected virtual void MyLateUpdate()
        {

        }

        /// <summary> Is called before the MonoBehaviour.Destroy() method is called. Hence the object this is on has not been officially destroyed yet. </summary>
        protected virtual void MyDestroy()
        {

        }
        #endregion Virtual methods for MyBehaviour



        #region Helper methods for inheriting classes

        /// <summary> Checks whether there is a collider on the object and that the collider is a trigger. Changes the collider to trigger if it is not the case. </summary>
        protected void m_EnsureGameObjectHasTrigger()
        {
            Collider coll = GetComponent<Collider>();
            if (coll == null)
            {
                Debugger.LogError($"There is no collider component on gameObject named \"{gameObject.name}\".");
                return;
            }

            if (coll.isTrigger == false)
            {
                Debug.LogWarning($"The collider on gameObject named \"{gameObject.name}\" should be a trigger. Changing it now but it should also be done when out of playmode.");
                coll.isTrigger = true;
            }
        }

        /// <summary> Logs a -Property not assigned- warning message if the condition is TRUE. Returns the result of the condition. </summary>
        /// <typeparam name="T"> The component on which the property is. </typeparam>
        /// <param name="condition"> Condition that checks whether the target property is null. </param>
        protected bool m_PropertyIsNull<T>(bool condition, string propertyName)
        {
            if (condition == false)
                return false;

            Debugger.LogWarning($"The \"{propertyName}\" property of the component \"{typeof(T)}\" on the object \"{gameObject.name}\", inside the object \"{gameObject.GetMainObject().name}\" is null or not assigned in the inspector.");
            return true;
        }

        /// <summary>Tries to fetch a target component of type T on the current object, if no such component can be found an error message is logged.</summary>
        /// <typeparam name="T"> The type of component to fetch for. </typeparam>
        /// <param name="targetObj"> If provided, will fetch for the target component on that object. By default the target component will be fetched on the current object. </param>
        /// <returns>Component of type T if it has been found. Otherwise returns null.</returns>
        protected T m_FetchForComponent<T>(GameObject targetObj = null)
        {
            if (targetObj == null)
                targetObj = gameObject;

            var targetComponents = targetObj.GetComponents<T>();
            if (targetComponents.Length == 0)
                Debug.LogError($"The object \"{targetObj.name}\" does not have a component of type \"{typeof(T)}\".");
            
            if (targetComponents.Length > 1)
                Debug.LogError($"The object \"{targetObj.name}\" has more than 1 component of type \"{typeof(T)}\". Only the first one in the list will be returned. Check the behaviour on WorldObject \"{worldName}\" to account for more than 1 component.");
            if (targetComponents.Length > 0)
                return targetComponents[0];
            else
                return default(T);
        }

        /// <summary>Tries to fetch a target component of type T in the child objects of the current object, if no such component can be found an error message is logged.</summary>
        /// <typeparam name="T"> The type of component to fetch for. </typeparam>
        /// <param name="targetObj"> If provided, will fetch for the target component in the children of that object. By default the target component will be fetched in the child objects of the current object. </param>
        /// <returns>Component of type T if it has been found. Otherwise returns null.</returns>
        protected T m_FetchForComponentInChildren<T>(GameObject targetObj = null)
        {
            if (targetObj == null)
                targetObj = gameObject;

            var targetComponent = targetObj.GetComponentInChildren<T>();
            if (targetComponent == null)
            {
                Debug.LogError($"None of the child objects of the object \"{targetObj.name}\" has a component of type \"{typeof(T)}\".");
                return default(T);
            }
            return targetComponent;
        }

        /// <summary>Tries to fetch the target components of type T in the child objects of the current object, if no such components can be found an error message is logged.</summary>
        /// <typeparam name="T"> The type of components to fetch for. </typeparam>
        /// <param name="targetObj"> If provided, will fetch for the target components in the children of that object. By default the target components will be fetched in the children of the current object. </param>
        /// <returns>List of components of type T if they have been found. Otherwise returns null.</returns>
        protected List<T> m_FetchForComponentsInChildren<T>(GameObject targetObj = null)
        {
            if (targetObj == null)
                targetObj = gameObject;

            T[] targetComponents = targetObj.GetComponentsInChildren<T>();
            if (targetComponents.Length == 0)
            {
                Debug.LogError($"None of the child objects of the object \"{targetObj.name}\" has a component of type \"{typeof(T)}\".");
                return null;
            }
            return targetComponents.ToList();
        }

        #endregion


        #region Custom inspector methods
        /// <summary> Allows the inspector to show warnings. </summary>
        public virtual List<InspectorMessage> GetInspectorWarnings()
        {
            return new List<InspectorMessage>();
        }
        /// <summary> Allows the inspector to show help messages. </summary>
        public virtual List<InspectorMessage> GetInspectorHelpMessages()
        {
            return new List<InspectorMessage>();
        }
        #endregion Custom inspector methods
    }
}