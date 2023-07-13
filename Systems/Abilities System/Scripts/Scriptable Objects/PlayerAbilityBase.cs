using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    public class PlayerAbilityBase : ScriptableObject
    {
        [Header("General Settings")]
        public string actionName = "Default Player Ability";
        public InputType inputType = InputType.Primary;
        public float m_abilityCooldownTime = 1f;
        public List<GameObject> HUDComponents = new List<GameObject>();

        protected bool m_abilityIsOnCooldown = false;
        protected float m_cooldownThreshold = 0f;
        private bool _cooldownStarted = false;

        protected PlayerInputController m_inputController;
        ///<summary>The gameobject of the entity that casted the ability.</summary>
        protected GameObject m_casterGameObject { get; set; }
        protected AbilityCaster m_abilityCaster { get; set; }

        ControlledInputAction inputAction;
        List<GameObject> _linkedHUDComponents = new List<GameObject>();

        #region Public accessors and events

        public delegate void OnAbilityUsed();
        public OnAbilityUsed onAbilityUsed { get; set; }

        public virtual AbilityType AbilityType => AbilityType.Unassigned;

        #endregion

        public void InitializeAbility(GameObject callerObject, AbilityCaster abilityCaster, PlayerInputController inputController)
        {
            m_casterGameObject = callerObject;
            m_abilityCaster = abilityCaster;
            m_inputController = inputController;

            m_ActionInitialized();
            _RegisterAbilityToInput();

            //Create HUD components
            foreach (var item in HUDComponents)
            {
                _linkedHUDComponents.Add(Managers.uiManager.InstantiateNewHUDComponent(item));
            }
        }

        /// <summary> Called when the ability is removed from the player or when the player dies. </summary>
        public void OnAbilityDestruction()
        {
            //Destroy the linked HUD components
            foreach (var item in _linkedHUDComponents)
            {
                Managers.uiManager.RemoveHUDComponent(item);
            }

            //Deregister form the input
            DeregisterActionFromInput();
        }

        #region General methods and their virtual version

        public void UpdateLoop()
        {
            m_Update();

            if (m_IsOnCooldown(out float remainingTime))
            {
                m_WhileOnCoolDown(remainingTime);
            }
            else if (_cooldownStarted)
            {
                _cooldownStarted = false;
                m_OnCooldownEnded();
            }
        }
        public void FixedUpdateLoop()
        {
            m_FixedUpdate();
        }
        public void LateUpdateLoop()
        {
            m_LateUpdate();
        }
        public void OnAnimationEvent(string eventIdentifier)
        {
            m_OnAnimationEvent(eventIdentifier);
        }
        public void OnCollisionEnter(Collision collision)
        {
            m_OnCollisionEnter(collision);
        }

        protected virtual void m_ActionInitialized()
        {

        }
        protected virtual void m_Update()
        {

        }
        protected virtual void m_FixedUpdate()
        {

        }
        protected virtual void m_LateUpdate()
        {

        }
        protected virtual void m_OnAnimationEvent(string eventIdentifier)
        {

        }
        protected virtual void m_OnCollisionEnter(Collision collision)
        {

        }

        #endregion General methods and their virtual version


        #region Charges amount

        public int GetChargesAmount()
        {
            return m_GetChargesAmount();
        }

        protected virtual int m_GetChargesAmount()
        {
            Debug.LogError("This should be overwriten");
            return 0;
        }

        #endregion


        #region Cooldown methods

        protected void m_StartCooldown()
        {
            m_cooldownThreshold = _GetNewCooldownThreshold(m_abilityCooldownTime);
            _cooldownStarted = true;
        }

        protected virtual void m_OnCooldownStart(float cooldownTime)
        {

        }

        protected virtual void m_WhileOnCoolDown(float remainingTime)
        {
            
        }

        protected virtual void m_OnCooldownEnded()
        {

        }

        protected virtual bool m_IsOnCooldown(out float remainingTime)
        {
            if (Time.realtimeSinceStartup < m_cooldownThreshold)
            {
                remainingTime = m_cooldownThreshold - Time.realtimeSinceStartup;
                return true;
            }
            remainingTime = 0f;
            return false;
        }

        protected virtual float _GetNewCooldownThreshold(float coolDownTime)
        {
            return Time.realtimeSinceStartup + coolDownTime;
        }


        #endregion Dooldown methods


        #region Inputs

        private void _RegisterAbilityToInput()
        {
            inputAction = m_inputController.GetActionOfType(inputType);
            inputAction.started += m_ActionStarted;
            inputAction.performed += m_ActionPerformed;
            inputAction.canceled += m_ActionCanceled;
            inputAction.onInputDisabled += m_OnInputDisabled;
            inputAction.onInputEnabled += m_OnInputEnabled;
        }
        public void DeregisterActionFromInput()
        {
            inputAction.DeregisterFromInput();
        }

        protected virtual void m_OnInputEnabled()
        {

        }

        protected virtual void m_OnInputDisabled()
        {

        }

        protected virtual void m_ActionStarted(ControlledInputContext context)
        {
            
        }
        protected virtual void m_ActionPerformed(ControlledInputContext context)
        {
            onAbilityUsed?.Invoke();
        }
        protected virtual void m_ActionCanceled(ControlledInputContext context)
        {

        }

        #endregion
    }
}