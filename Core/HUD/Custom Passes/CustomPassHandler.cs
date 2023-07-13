using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace PropellerCap
{
    public class CustomPassHandler : MonoBehaviour
    {
        [SerializeField] bool _disableAllOnStart = true;

        CustomPassVolume _volume;

        List<CustomPass> _activePasses = new List<CustomPass>();

        private void Awake()
        {
            _volume = GetComponent<CustomPassVolume>();
            if (_volume == null)
            {
                Debug.LogError($"There is no {typeof(CustomPassVolume)} component on the custom pass handler.");
            }
        }

        void Start()
        {
            HUD.customPassHandler = this;
            Managers.uiManager.customPassHandler = this;
            
            if (_disableAllOnStart)
            {
                foreach (CustomPass item in _volume.customPasses)
                {
                    item.enabled = false;
                }
            }
        }

        public void EnableCustomPass(CustomPassType pass, CustomPassLoadMode loadMode = CustomPassLoadMode.Additive)
        {
            _LoadCustomPass(_GetPass(pass), loadMode);
        }

        public void DisableCustomPass(CustomPassType pass)
        {
            CustomPass targetPass = _GetPass(pass);

            if (targetPass == null) 
                return;            

            if (_activePasses.Contains(targetPass) == false)
            {
                Debugger.LogWarning($"Custom pass {pass} is not in active passes list. It might already have been disabled.");
                return;
            }

            targetPass.enabled = false;
            _activePasses.Remove(targetPass);
        }

        public void DisableAllCustomPasses()
        {
            foreach (CustomPass item in _activePasses)
            {
                item.enabled = false;
            }
            _activePasses.Clear();
        }

        public Material GetPassMaterial(CustomPassType pass, bool enablePassIfNotAlreadyActive = true, CustomPassLoadMode loadMode = CustomPassLoadMode.Additive)
        {
            CustomPass customPass = GetCustomPass(pass, enablePassIfNotAlreadyActive, loadMode);

            if (customPass is not FullScreenCustomPass)
            {
                Debugger.LogError($"The custom pass \"{pass}\" is not a fullScreenCustom pass.");
                return null;
            }

            return ((FullScreenCustomPass)customPass).fullscreenPassMaterial;
        }

        public CustomPass GetCustomPass(CustomPassType pass, bool enablePassIfNotAlreadyActive = true, CustomPassLoadMode loadMode = CustomPassLoadMode.Additive)
        {
            CustomPass targetPass = _GetPass(pass);

            if (targetPass == null)
                return null;

            if (enablePassIfNotAlreadyActive && _activePasses.Contains(targetPass) == false)
            {
                EnableCustomPass(pass, loadMode);
            }

            return targetPass;
        }

        private CustomPass _GetPass(CustomPassType pass)
        {
            string passName = pass.ToString();
            foreach (CustomPass item in _volume.customPasses)
            {
                if (item.name == passName)
                    return item;
            }
            Debugger.LogError($"The custom pass name {passName} could not be found in the custom passes. Make sure there is a custom pass named : {passName}");
            return null;
        }

        private void _LoadCustomPass(CustomPass pass, CustomPassLoadMode loadMode)
        {
            //Disable other passes based on load mode
            if (loadMode == CustomPassLoadMode.Single)
            {
                foreach (CustomPass item in _activePasses)
                {
                    item.enabled = false;
                }
                _activePasses.Clear();
            }

            if (pass == null)
                return;

            //Enable the new custom pass
            pass.enabled = true;

            //Add custom pass to keep track of it
            _activePasses.Add(pass);
        }
    }
}
