using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class UIManager : ManagerBase
    {
        public static UIManager Instance { get; private set; }


        public FlareChargesVisualizer flareChargesVisualizer { get; set; }
        public HookCrosshair hookCrossair { get; set; }
        public SanityBar sanityBar { get; set; }
        public CustomPassHandler customPassHandler { get; set; }
        public BlackFader blackFader { get; set; }

        public HUDContainersHandler hudContainersHandler { get; set; }

        List<HUDComponent> _HUDComponents = new List<HUDComponent>();

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
            Debugger.LogInit("Init in UI Manager");
            Managers.uiManager = this;
            HUD.uiManager = this;
        }
        public override void MyAwake()
        {
            Debugger.LogInit("MyAwake in UI Manager");
        }
        public override void MyStart()
        {
            Debugger.LogInit("MyStart in UI Manager");
        }
        public override void OnGamePreparationComplete()
        {
            HideAllHUDComponents();
        }
        public override bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            existingManager = FindObjectOfType<UIManager>();

            if (existingManager == null)
                return false;

            if (existingManager.GetType() != this.GetType())
                Debug.LogError("Check manager name ! Be careful when copy-pasting ;)" +
                    " A " + existingManager.GetType().ToString() +
                    " is not a " + this.GetType().ToString());

            return existingManager != null;
        }
        #endregion Initialization




        #region UI Components controls

        public void HideAllHUDComponents()
        {
            foreach (var item in _HUDComponents)
            {
                item.HideOnInitialization();
            }
        }

        public void ShowAllHUDComponents()
        {
            foreach (var item in _HUDComponents)
            {
                item.ShowComponent();
            }
        }

        public GameObject InstantiateNewHUDComponent(GameObject newHUDComponentPrefab)
        {
            HUDComponent hudComp = newHUDComponentPrefab.GetComponent<HUDComponent>();
            if (hudComp == null)
            {
                Debugger.LogError($"There is no Component that inherits from \"{nameof(HUDComponent)}\" on the object \"{newHUDComponentPrefab}\".");
                return null;
            }
            
            //Only instantiate the prefab if it has a HUDComponent
            GameObject hudObj = Instantiate(newHUDComponentPrefab, hudContainersHandler.GetContainer(hudComp));
            hudComp = hudObj.GetComponent<HUDComponent>();
            hudComp.OnComponentInstantiation();
            return hudObj;
        }

        /// <summary> Destroys the given HUDComponent object. </summary>
        public void RemoveHUDComponent(GameObject hudObject)
        {
            HUDComponent hudComp = hudObject.GetComponent<HUDComponent>();
            if (hudComp == null)
            {
                Debugger.LogError($"There is no Component that inherits from \"{nameof(HUDComponent)}\" on the object \"{hudObject}\".");
                return;
            }

            hudComp.OnComponentDestruction();
            Destroy(hudObject);
        }

        public void RegisterHUDComponent(HUDComponent hudComponent)
        {
            if (_HUDComponents.Contains(hudComponent))
            {
                Debugger.LogError($"UIManager already contains the HUD Component : {hudComponent.gameObject.name}");
                return;
            }
            _HUDComponents.Add(hudComponent);
        }

        public void DeregisterHUDComponent(HUDComponent hudComponent)
        {
            if (_HUDComponents.Contains(hudComponent) == false)
            {
                Debugger.LogError($"UIManager does not contain the HUD Component : {hudComponent.gameObject.name}");
                return;
            }
            _HUDComponents.Remove(hudComponent);
        }
        #endregion UI Components controls 

    }
}
