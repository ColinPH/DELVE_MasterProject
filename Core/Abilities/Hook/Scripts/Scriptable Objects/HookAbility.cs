using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    public enum HookType
    {
        Unassigned = 0,
        Swing = 1,
        Fast = 2
    }
    public enum HookState
    {
        Retracted = 0,
        ShootAnimation = 1,
        WaitForExtension = 2,
        WaitForRetract = 3,
        RecoilAnimation = 4,
        Out = 5,
        /// <summary> When the hook is pulling the player. </summary>
        Pulling = 6
    }
    public enum CastType { Raycast = 0, SphereCast = 1, }
    public enum FastMovementType { Constant = 0, AnimationCurve = 1, }

    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Player Abilities/Hook Ability", order = 1)]
    public class HookAbility : PlayerAbilityBase
    {
        #region General hook settings
        [Header("Values")]
        /// <summary>Max raycast range when trying to hook to something.</summary>
        [SerializeField] float maxRange = 20f;
        /// <summary>Layermask for what is hookable.</summary>
        [SerializeField] LayerMask hookableMask;
        [SerializeField] LayerMask _validLayersMask;
        [SerializeField] CastType _castType = CastType.Raycast;
        [SerializeField] float _sphereCastRadius = 0.2f;
        [SerializeField] float _ropeExtensionSpeed = 5;
        [SerializeField] float _ropeRetractationSpeed = 5;
        [Header("Input")]
        /// <summary>The amount of time within which the player should release the button to start the fast hook action.</summary>
        [SerializeField, Range(0.1f, 0.7f)] float fastInputTime = 0.2f;
        /// <summary>The button that determines whether the player is anchored. (Anchored is used for applying the pull action)</summary>
        [SerializeField] InputType anchoredButton = InputType.SecondaryAlternative;
        /// <summary>The button that determines whether the player shortening the rope when swinging.</summary>
        [SerializeField] InputType sprintButton = InputType.PrimaryAlternative;

        [Header("Extend Animation")]
        [Tooltip("Identifier used to compare with the simple animation event caller. This identifier should match the identifier used in the S.A.E.C. for when the rope extension animation fires the event. ")]
        [SerializeField] string _extendRopeAnimEventIdentifier = "Extend rope";
        [Tooltip("Name of the trigger parameter used in the player animator controller to start the extend rope animation.")]
        [SerializeField] string _extendRopeAnimTrigger = "Extend rope";

        [Header("Recoil Animation")]
        [Tooltip("Name of the trigger parameter used in the player animator controller to start the recoil animation for when the rope has been retracted.")]
        [SerializeField] string _recoilAnimTrigger = "Recoil";

        [Header("Visual Effects")]
        [SerializeField] VisualEffectClip _extendVFXClip;
        [SerializeField] VisualEffectClip _recoilVFXClip;
        [SerializeField] VisualEffectClip _impactVFXClip;
        [SerializeField] VisualEffectClip _impactDecalVFXClip;

        [Header("Sounds")]
        [SerializeField] SoundClip _extendFailClip;
        [SerializeField] SoundClip _extendClip;
        [SerializeField] SoundClip _latchFailClip;
        [SerializeField] SoundClip _latchSuccessClip;
        [SerializeField] SoundClip _tightenRopeClip;
        [SerializeField] SoundClip _detachClip;
        [SerializeField] SoundClip _pullActionClip;
        [SerializeField] SoundClip _recoilClip;
        [SerializeField] SoundClip _flyMovementClip;
        [SerializeField] SoundClip _enterSwingingClip;
        [SerializeField] SoundClip _swingingClip;
        [SerializeField] SoundClip _faceplantClip;
        #endregion

        #region FAST hook settings
        [Space(20)]
        [Header("FAST hook settings")]
        /// <summary> Amount of force applied once when pulling on an object. </summary>
        [SerializeField] float _pullActionForce = 10f;
        [SerializeField] float _travelSpeed = 15f;
        [SerializeField] FastMovementType _movementType = FastMovementType.Constant;
        [SerializeField] AnimationCurve _travelCurve;
        [SerializeField] float _curveMultiplier = 1f;
        [Tooltip("Distance between gun and hook point at which the hook detaches itself.")]
        [SerializeField] float _gunUnhookDistance = 3f;
        [SerializeField, Tooltip("Applies only when fast hook goes to completion. Continues the movement further to make sure edges are reached and the velocity boost stops while the entity is on the edge.")]
        bool _applyMovementProjection = false;
        [Tooltip("How long should the projection be.")]
        [SerializeField] float _projectionAmount = 0.3f;
        [Tooltip("Value multiplying the velocity of the player when the fast hook goes to completion, does not apply when fast hook is interupted.")]
        [SerializeField] float _speedMultiplierAtDestination = 0.2f;
        [SerializeField] float _verticalOffset = 1.4f;
        [Tooltip("Amount of raycasts around the impact point, should be at least 4.")]
        [SerializeField] int _edgeDetectionResolution = 8;
        [SerializeField] float _edgeDetectionRadius = 0.3f;
        [Tooltip("Distance above impact point to which the travel destination point can be adjusted.")]
        [SerializeField] float _edgeDetectionHeight = 1.4f;

        //Moving entity to target
        bool _useHookableAsDestination = false;
        Vector3 _moveStartPos = new Vector3();
        Vector3 _moveDestinationPos = new Vector3();
        bool _foundDestinationWithDownCast = false;
        /// <summary>Used to keep track of the remaining distance and make sure it doesn't get bigger.</summary>
        float _previousRemainingDistance = 1000000f;

        //Swing timing properties
        float _fastHookStartTime = 0f;
        float _validFastHookTravelTime = 0.5f;
        #endregion

        #region SWING hook settings
        [Space(20)]
        [Header("SWING hook settings")]
        /// <summary> Amount of force applied once when the pull action starts. </summary>
        [SerializeField] float _initialPullForce = 10f;
        /// <summary> Amount of force applied over time when pulling on an object. </summary>
        [SerializeField] float _tensionPullForce = 2f;
        [SerializeField] float _startImpulseIntensity = 5f;
        [Tooltip("If the player walks with the rope extended, when the length of the rope passed this value, there will be feedback.")]
        [SerializeField] float _ropeMaxRangeWarningThreshold = 17f;
        [Header("Spring joint settings")]
        [SerializeField] float _spring = 4.5f;
        [SerializeField] float _damper = 7f;
        [SerializeField] float _massScale = 4.5f; 
        [Header("Rope length change speeds")]
        [SerializeField] float _ropeShorteningSpeed = 2f;
        [SerializeField] float _ropeLengtheningSpeed = 2f;

        //Rope control variables
        bool _isShorteningRope = false;
        bool _isLengtheningRope = false;
        /// <summary> Whether we should check if the rope is past its max range when the player walks with it. </summary>
        bool _checkRopeExtensionPastMaxRange = false;
        bool _hasPassedMaxRangeWarning = false; 

        //Joint variables
        bool _continuouslyUpdateJointAnchor = false;

        //Pull actions
        bool _isPullingOnRope = false;

        //Swing timing properties
        float _swingStartTime = 0f;
        float _validSwingHookTravelTime = 0.5f;
        #endregion

        #region Public accessors and events
        public override AbilityType AbilityType => AbilityType.Hook;

        public delegate void OnHookCooldownStart(float cooldownDuration);
        /// <summary> Called when the ability has been used. </summary>
        public OnHookCooldownStart onHookCooldownStart { get; set; }
        public delegate void OnHookCoollingdown(float remainingTime, float maxTime);
        /// <summary> Called each frame while the ability is on cooldown. </summary>
        public OnHookCoollingdown onHookCoollingdown { get; set; }
        public delegate void OnHookCooldownEnded();
        /// <summary> Called when the hook can be used again. </summary>
        public OnHookCooldownEnded onHookCooldownEnded { get; set; }

        #endregion

        //Ability functionality
        private FirstPersonCharacterController m_fpController { get; set; }
        private PlayerCharacter m_player { get; set; }
        //Hook functionality
        private GameObject m_hookProjectileObject { get; set; }
        private SpringJoint m_hookJoint { get; set; }
        //State and type
        private HookType m_hookType { get; set; }
        private HookState m_hookState { get; set; }
        //Impact information
        /// <summary> Main GameObject we are hooked to. Found using collider.gameObject.GetMainObject(). </summary>
        private GameObject m_hookedMainObject { get; set; }
        /// <summary> GameObject we are hooked to. Found using collider.gameObject. </summary>
        private GameObject m_hookedObject { get; set; }
        private RaycastHit m_impactHitPoint { get; set; }
        private Hookable m_hookable { get; set; }

        ///<summary>Whether the player is in the hooked state.</summary>
        private bool m_isHooked = false;
        ///<summary>Whether the player is anchored. (Used to apply the pull action and going down the rope)</summary>
        private bool m_isAnchored = false;
        ///<summary>Whether the button has been pressed and released faster than the fast input time. TRUE should result in fast hook, FALSE should be swing hook.</summary>
        private bool m_isShortRelease = false;
        private bool m_shootAnimationAndExtensionComplete = false;
        ///<summary> If TRUE, will wait until the fast input time is over to start the swing hook. </summary>
        private bool m_waitingForFastInputTime = false;
        ///<summary>Whether the shot is in range and attaches to something hookable</summary>
        private bool m_shotIsValid = false;

        ControlledInputAction _anchoredButtonAction;
        ControlledInputAction _sprintButtonAction;
        ControlledInputAction _jumpButtonAction;
        Animator _animator;

        //Cooldown
        float _inputDownTime = 0f;

        //Rope extension
        float _lerpExtendValue = 0;

        //Rope retractation
        float _lerpRetractValue = 0;
        Vector3 _ropeRetractationStartPoint = new Vector3();

        //**************************************************
        // Overrides from anilitybase and monobehaviour-like methods
        //**************************************************

        #region PlayerAbilityBase overrides

        protected override void m_ActionInitialized()
        {
            base.m_ActionInitialized();
            m_hookType = HookType.Unassigned;
            m_hookState = HookState.Retracted;

            m_fpController = m_casterGameObject.GetComponent<FirstPersonCharacterController>();
            m_player = m_casterGameObject.GetComponent<PlayerCharacter>();
            //Register the anchored actions to the button
            _anchoredButtonAction = m_inputController.GetActionOfType(anchoredButton);
            _anchoredButtonAction.started += _OnAnchoredDown;
            _anchoredButtonAction.canceled += _OnAnchoredUp;
            //Register the sprint actions to the button
            _sprintButtonAction = m_inputController.GetActionOfType(sprintButton);
            _sprintButtonAction.started += _OnSprintDown;
            _sprintButtonAction.canceled += _OnSprintUp;
            //Register to the jump action
            _jumpButtonAction = m_inputController.GetActionOfType(InputType.Jump);
            _jumpButtonAction.started += _OnJump;

            _animator = m_abilityCaster.bodyAnimator;

            Debugger.LogHook("HookAbility initialized.");
        }

        protected override void m_ActionStarted(ControlledInputContext context)
        {
            base.m_ActionStarted(context);
            Debugger.LogHook("Ability button has been pressed.");

            //A button press while the fast hook is in use should stop the hook action
            if (m_hookState != HookState.Retracted)
            {
                if (m_hookState == HookState.Pulling && m_hookType == HookType.Fast)
                {
                    Debugger.LogHook("In fast hook state : stopping hook now.");
                    m_StopHooking("Because hook input has been activated again while fast hook was active.");
                    return;
                }
                Debugger.LogHook("Hookstate is not retracted, not pulling and hooktype is not fast : doing nothing.");
                return;
            }

            //If the button press was before the cooldown ended
            if (m_IsOnCooldown(out float remainingTime))
            {
                Debugger.LogHook("Still on cooldown : doing nothing.");
                m_OnCooldownNotEnded();
                Metrics.levelData.onCooldownHookShots += 1;
                return;
            }

            _inputDownTime = Time.realtimeSinceStartup;

            //Process the ability
            Debugger.LogHook("Processing hook ability.");
            m_ShootHook();
        }

        protected override void m_ActionCanceled(ControlledInputContext context)
        {
            base.m_ActionCanceled(context);
            Debugger.LogHook("Ability button has been released.");

            if (m_IsOnCooldown(out float remainingTime))
            {
                Debugger.LogHook("Released button while on cooldown : doing nothing.");
                return;
            }

            //If the button has been released earlier than the fast input time
            if (Time.realtimeSinceStartup < _inputDownTime + fastInputTime)
            {
                //Is short release
                Debugger.LogHook("Ability button released before fast input time. Should trigger fast hook.");
                m_isShortRelease = true;
                //This is for when the fast input time is longer than the animation takes to fire the event
                if (m_shootAnimationAndExtensionComplete && m_shotIsValid)
                {
                    Debugger.LogHook("Is a short release therefore starting FAST hook. Button released after animation has ended.");
                    m_StartFastHook();
                }
                return;
            }

            if (m_hookState == HookState.Out || m_hookState == HookState.Pulling)
            {
                //Is long release
                Debugger.LogHook("Ability button released after fast input time. Should stop swing hook.");
                m_isShortRelease = false;
                m_StopHooking("Because it was a long release.");
            }
        }

        protected override void m_FixedUpdate()
        {
            base.m_FixedUpdate();

        }

        protected override void m_Update()
        {
            base.m_Update();

            if (m_hookState == HookState.WaitForExtension)
                _ExtendRope();

            if (m_hookState == HookState.WaitForRetract)
                _RetractHook();

            _ControlCrosshair();
            _WaitForFastInputTime();

            _Update_SwingHook();
            _Update_FastHook();
        }

        protected override void m_LateUpdate()
        {
            base.m_LateUpdate();

        }

        /// <summary> General function of all abilities for when animation events need to call methods.</summary>
        protected override void m_OnAnimationEvent(string eventIdentifier)
        {
            if (eventIdentifier == _extendRopeAnimEventIdentifier)
                Anim_StartExtendRope();
        }

        protected override void m_OnCollisionEnter(Collision collision)
        {
            if (m_hookType == HookType.Fast)
                _OnCollisionEnter_FastHook(collision);
        }

        protected override void m_OnCooldownStart(float cooldownTime)
        {
            onHookCooldownStart?.Invoke(cooldownTime);
        }

        protected override void m_WhileOnCoolDown(float remainingTime)
        {
            onHookCoollingdown?.Invoke(remainingTime, m_abilityCooldownTime);
        }
        protected override void m_OnCooldownEnded()
        {
            onHookCooldownEnded?.Invoke();
        }

        #endregion PlayerAbilityBase overrides

        //**************************************************
        // Special input methods
        //**************************************************

        #region Special input button actions

        void _OnAnchoredDown(ControlledInputContext context)
        {
            _OnAnchoredDown_SwingHook(context);
            _OnAnchoredDown_FastHook(context);
        }

        void _OnAnchoredUp(ControlledInputContext context)
        {
            _OnAnchoredUp_SwingHook(context);
            _OnAnchoredUp_FastHook(context);
        }

        void _OnSprintDown(ControlledInputContext context)
        {
            _OnSprintDown_SwingHook(context);
            _OnSprintDown_FastHook(context);
        }

        void _OnSprintUp(ControlledInputContext context)
        {
            _OnSprintUp_SwingHook(context);
            _OnSprintUp_FastHook(context);
        }

        void _OnJump(ControlledInputContext context)
        {
            _OnJump_SwingHook(context);
            _OnJump_FastHook(context);
        }

        #endregion

        //**************************************************
        // General methods
        //**************************************************

        #region General methods
        public void m_OnCooldownNotEnded()
        {
            Debugger.LogHook("Hook is still on cooldown.");
            Sound.PlaySound(_extendFailClip, m_casterGameObject);
        }

        private void _ControlCrosshair()
        {
            Transform cameraTrans = m_abilityCaster.cameraTransform;
            RaycastHit hit;
            GameObject hookedMainObj;
            GameObject hookedObj;

            //TODO make the hook ability control the crosshair using the event system instead
            if (m_ShotIsValid(out hit, out hookedMainObj, out hookedObj, cameraTrans.position, cameraTrans.forward, maxRange, hookableMask, false))
            {
                if (HUD.crosshair != null)
                    HUD.crosshair.ShowGrappleIndicator();
            }
            else 
            {
                if (HUD.crosshair != null)
                    HUD.crosshair.HideGrappleIndicator();
            }
        }

        private void _WaitForFastInputTime()
        {
            if (m_waitingForFastInputTime)
            {
                //If we pass the fast input time
                if (Time.realtimeSinceStartup > _inputDownTime + fastInputTime)
                {
                    m_waitingForFastInputTime = false;
                    Debugger.LogHook("Starting SWING hook. From waiting for the fast input time to be over because animation ended earlier.");
                    m_StartSwingHook();
                }
            }
        }

        public void m_StopHooking(string stopReason)
        {
            Debugger.LogHook("Stop hooking and apply cooldown. " + stopReason);

            if (m_hookState == HookState.WaitForRetract || m_hookState == HookState.Retracted) 
                return;

            Sound.PlaySound(_detachClip, m_casterGameObject);

            //Update threshold to apply cooldown time
            m_StartCooldown();

            m_isHooked = false;

            if (m_hookType == HookType.Fast)
                _OnFastHookStop();

            if (m_hookType == HookType.Swing)
                _OnSwingHookStop();

            //Stop the shortening and lengthening of the rope
            _StopShorteningRope();
            _StopLengtheningRope();

            _StopMaintainedPullAction(); //Uses m_hookable
            _isPullingOnRope = false;


            //Detach the hook from the hookable object
            if (m_hookable != null)
                m_hookable.OnHookDettached();
            m_hookable = null;
            m_hookedMainObject = null;

            m_hookType = HookType.Unassigned;
            m_isShortRelease = false;
            m_shootAnimationAndExtensionComplete = false;
            m_waitingForFastInputTime = false;

            //Reset the collider of the character
            m_fpController.SetColliderTypeTo(ColliderType.Normal);

            //Reset the state of the character
            m_player.stateMachineBehaviour.TransitionToNewState<IdleState>();

            m_StartHookRetractProcess();
        }

        #endregion

        //**************************************************
        // Hook shooting, extension, retractation and tightening
        //**************************************************

        #region Hook shooting

        protected virtual void m_ShootHook()
        {
            //Save whether the shot is valid, and save the impact information
            Transform cameraTrans = m_abilityCaster.cameraTransform;
            RaycastHit hit;
            GameObject mainObj;
            GameObject obj;
            m_shotIsValid = m_ShotIsValid(out hit, out mainObj, out obj, cameraTrans.position, cameraTrans.forward, maxRange, hookableMask);

            m_impactHitPoint = hit;

            if (m_shotIsValid)
            {
                m_hookedMainObject = mainObj;
                m_hookedObject = obj;
                //Find the hookable component is there is one
                m_hookable = m_hookedMainObject.GetComponent<Hookable>(); //There should not always be a Hookable component
                if (m_hookable != null)
                    m_hookable.HookRaycastShot(m_impactHitPoint);
                Debugger.LogHook("The shot is valid, hooked object is : " + m_hookedMainObject + " hookable is " + m_hookable);
            }

            _StartShootAnimation();
        }

        private void _StartShootAnimation()
        {
            Debugger.LogHook("Trigger the shoot animation.");
            m_hookState = HookState.ShootAnimation;

            m_shootAnimationAndExtensionComplete = false;

            _animator.SetTrigger(_extendRopeAnimTrigger);
            //Anim_StartExtendRope() will be called by the animation event after X frames
        }

        #endregion

        #region Hook extension

        public void Anim_StartExtendRope()
        {
            //If the hook has been stopped while the animation was playing, do nothing
            /*if ()
                return;*/

            Debugger.LogHook("Animation event has started the rope extension.");
            Sound.PlaySound(_extendClip, m_casterGameObject);

            m_hookProjectileObject = m_abilityCaster.hookGun.DetachHook().gameObject;

            _lerpExtendValue = 0f;

            m_hookState = HookState.WaitForExtension;
        }

        private void _ExtendRope()
        {
            Vector3 startPos = m_GetGunTip().position;
            Vector3 endPos = m_GetHookAttachPoint(m_hookable, m_impactHitPoint);

            m_hookProjectileObject.transform.position = Vector3.Lerp(startPos, endPos, _lerpExtendValue);
            _lerpExtendValue += _ropeExtensionSpeed * Time.deltaTime;

            if (_lerpExtendValue >= 0.98)
            {
                //Hook has arrived at destination
                _RopeExtensionEnded();
            }
        }

        private void _RopeExtensionEnded()
        {
            m_shootAnimationAndExtensionComplete = true;

            if (m_shotIsValid == false)
            {
                //If not in range
                Debugger.LogHook("Not in range for latching.");
                Sound.PlaySound(_latchFailClip, m_casterGameObject);
                Metrics.levelData.failedHookLatches += 1;

                m_StartHookRetractProcess();
                return;
            }

            //If within range
            Debugger.LogHook("In range for latching.");
            m_hookState = HookState.Out;
            _Latch();
        }

        private void _Latch()
        {
            Debugger.LogHook("Latch succeeded.");
            Metrics.levelData.successfulHookLatches += 1;
            //The hook is now attached to the Hookable object if the object we latch on to is hookable
            if (m_hookable!= null)
                m_hookable.OnHookVisuallyAttached();

            //Fetch the material type of the object we hit
            MaterialType matType = m_hookedObject.GetMaterialType();
            Sound.PlaySound(_latchSuccessClip, m_hookProjectileObject, matType);

            VisualEffects.SpawnVFX(_impactVFXClip, m_impactHitPoint.point, m_impactHitPoint.normal);
            
            VisualEffects.SpawnVFX(_impactDecalVFXClip, m_impactHitPoint.point, m_impactHitPoint.normal);

            _StartTighteningRope();
        }

        #endregion

        #region Hook retract process

        public void m_StartHookRetractProcess()
        {
            Debugger.LogHook("Start retracting rope.");
            //_ropeRetractationStartPoint = m_GetHookAttachPoint(m_hookable, m_impactHitPoint);
            _ropeRetractationStartPoint = m_hookProjectileObject.transform.position;

            //Apply the cooldown time
            m_StartCooldown();

            _lerpRetractValue = 0f;

            m_isShortRelease = false;
            //This will make the update method call _RetracHook()
            m_hookState = HookState.WaitForRetract;
        }

        private void _RetractHook()
        {
            Vector3 startPos = _ropeRetractationStartPoint;
            Vector3 endPos = m_GetGunTip().position;

            m_hookProjectileObject.transform.position = Vector3.Lerp(startPos, endPos, _lerpRetractValue);
            _lerpRetractValue += _ropeRetractationSpeed * Time.deltaTime;

            if (_lerpRetractValue >= 0.98)
            {
                //Hook has arrived at destination
                _HookRetracted();
            }
        }

        private void _HookRetracted()
        {
            Debugger.LogHook("Rope retracted.");
            Sound.PlaySound(_recoilClip, m_casterGameObject);

            VisualEffects.SpawnVFX(_recoilVFXClip, m_GetGunTip());

            m_abilityCaster.hookGun.AttachHook(m_hookProjectileObject.transform);

            _animator.SetTrigger(_recoilAnimTrigger);

            m_hookState = HookState.Retracted;
        }

        #endregion

        #region Hook tightening

        private void _StartTighteningRope()
        {
            Debugger.LogHook("Start tightening rope.");

            //TODO tighten the rope
            _RopeIsTightened();
        }

        private void _RopeIsTightened()
        {
            Debugger.LogHook("Rope is tightened.");

            _InitiateHookActionBasedOnTime();
        }

        private void _InitiateHookActionBasedOnTime()
        {
            Debugger.LogHook("Activating specific hook based on release time.");

            //If the button has been released before the animation has ended and it is a short release
            if (m_isShortRelease && m_hookType == HookType.Unassigned)
            {
                //Start fast hook
                Debugger.LogHook("Is a short release therefore starting FAST hook. Button released before animation has ended.");
                m_StartFastHook();
            }
            //To start swinging we need to have passed the fast input time
            else if (Time.realtimeSinceStartup > _inputDownTime + fastInputTime)
            {
                Debugger.LogHook("Starting SWING hook. From the animation and because we have passed the fast input time.");
                m_StartSwingHook();
            }
            else
            {
                m_waitingForFastInputTime = true;
            }
        }

        #endregion

        //**************************************************
        // Spefic hook behaviours
        //**************************************************

        #region Specific hook behaviours

        //**************************************************
        // Fast hook
        //**************************************************

        #region Fast hook behaviour

        private void m_StartFastHook()
        {
            Debugger.LogHook("Fast hook has started");
            Metrics.levelData.successfulFastHookUses += 1;
            _fastHookStartTime = Time.time;
            m_hookType = HookType.Fast;

            if (m_isAnchored || m_hookable != null)
            {
                _SinglePullAction(_pullActionForce, m_hookable, m_impactHitPoint);

                //if (m_isAnchored)
                    return;
            }

            _PreparePlayerLaunch();
        }

        private void _OnFastHookStop()
        {
            float fastHookTravelTime = Time.time - _fastHookStartTime;
            if (fastHookTravelTime > 0.001f) //In case the hook activates something then it has a travel time of 0. and shouldn't count
                Metrics.levelData.fastHookDurationTimes.Add(fastHookTravelTime);
            if (fastHookTravelTime < _validFastHookTravelTime)
                Metrics.levelData.successfulFastHookTravels += 1;

            //This is for the Fast hook
            _useHookableAsDestination = false;
            m_fpController.SetGravityTo(true);
        }

        private void _OnCollisionEnter_FastHook(Collision collision)
        {
            if (m_hookState == HookState.Pulling)
            {
                Sound.PlaySound(_faceplantClip, m_abilityCaster.gameObject);
                Metrics.levelData.fastHookStopByCollision += 1;

                m_StopHooking("Because faceplant.");
            }
        }

        void _Update_FastHook()
        {
            if (m_hookState == HookState.Pulling)
                _MovePlayer();
        }

        private void _PreparePlayerLaunch()
        {
            _moveStartPos = m_GetGunTip().position;
            _foundDestinationWithDownCast = false;

            if (m_hookable == null)
                _moveDestinationPos = _FindValidDestinationAroundImpact(m_impactHitPoint, _edgeDetectionResolution, _edgeDetectionRadius, _edgeDetectionHeight, hookableMask);
            else
            {
                _useHookableAsDestination = true;
                _moveDestinationPos = _GetDestinationFromHookable();
            }

            Debugger.LogHook("Found destination point with down raycast : " + _foundDestinationWithDownCast);

            _moveDestinationPos += new Vector3(0, _verticalOffset, 0);
            _previousRemainingDistance = 1000000f; //Makesure the first time we compare it the previous distance is larger

            //To make sure we reach the top of the edge
            Vector3 direction = _moveDestinationPos - _moveStartPos;
            if (_applyMovementProjection)
                _moveDestinationPos += direction.normalized * _projectionAmount;

            _LaunchPlayer();
        }

        private void _LaunchPlayer()
        {
            m_fpController.SetColliderTypeTo(ColliderType.Swing);
            Sound.PlaySound(_pullActionClip, m_abilityCaster.gameObject);

            m_hookState = HookState.Pulling;
            m_isHooked = true;

            m_player.stateMachineBehaviour.TransitionToNewState<GrapplingState>();
            m_fpController.SetGravityTo(false);
            Sound.PlaySound(_flyMovementClip, m_abilityCaster.gameObject);
        }

        private void _MovePlayer()
        {
            Vector3 startPos = _moveStartPos;
            Vector3 endPos;
            if (_useHookableAsDestination)
            {
                startPos = m_GetGunTip().position;
                endPos = _GetDestinationFromHookable();
            }
            else
                endPos = _moveDestinationPos;
            //TODO add this to the debug component
            //Debug.DrawLine(m_abilityCaster.transform.position, m_abilityCaster.transform.position + m_fpController.Velocity().normalized * 10, Color.blue);
            //Debug.DrawLine(startPos, endPos, Color.red, 0.1f);

            //Sample the movement curve based on the traveled distance
            float distTraveled = Vector3.Distance(startPos, m_abilityCaster.transform.position);
            float sampleIndex = distTraveled / maxRange;

            Vector3 movementDirection = endPos - startPos;
            if (_movementType == FastMovementType.Constant)
                m_fpController.SetVelocity(movementDirection.normalized * _travelSpeed);
            else if (_movementType == FastMovementType.AnimationCurve)
                m_fpController.SetVelocity(movementDirection.normalized * _travelCurve.Evaluate(sampleIndex) * _curveMultiplier);

            //This change makes it so that the fast hook doesn't stop when the player rotates quickly
            //float distToDest_Gun = Vector3.Distance(m_GetGunTip().position, endPos);
            float distToDest_Gun = Vector3.Distance(m_abilityCaster.transform.position, endPos);

            //Debug.DrawLine(m_GetGunTip().position, endPos, Color.green);
            //Debug.DrawLine(m_abilityCaster.transform.position, endPos - new Vector3(0f, _verticalOffset, 0f), Color.cyan);

            bool gunUnhook = distToDest_Gun < _gunUnhookDistance;
            bool overshooting = _previousRemainingDistance < distToDest_Gun && Mathf.Abs(distToDest_Gun - _previousRemainingDistance) > 0.05f;
            
            if (gunUnhook || overshooting)
            {
                //Entity at destinationm
                _PlayerArrived();
            }

            _previousRemainingDistance = distToDest_Gun;
        }

        private void _PlayerArrived()
        {
            //Drastically reduce the speed if destination is reached, does not apply if stopped mid air
            m_fpController.SetVelocity(m_fpController.Velocity() * _speedMultiplierAtDestination);

            m_StopHooking("Because the player has arrived from the fast hook.");
        }

        private Vector3 _GetDestinationFromHookable()
        {
            return m_hookable.GetHookAttachmentPoint();
        }

        private Vector3 _FindValidDestinationAroundImpact(RaycastHit impactHit, int amountCasts, float radius, float verticalOffest, LayerMask castMask)
        {
            //Find the starting positions of the raycasts around the impact point
            List<Vector3> startPos = new List<Vector3>();

            if (amountCasts < 4)
                Debugger.LogWarning("Fast hook edge detection resolution is lower than 4, it should be at least 4, for best result use 8.");

            //Calculate the starting positions around the impact point
            float angle = 360f / amountCasts;
            for (int i = 0; i < amountCasts; i++)
            {
                Vector3 pos = impactHit.point;
                pos.x += Mathf.Cos(angle * i * Mathf.Deg2Rad) * radius;
                //pos.y += verticalOffest;
                pos.z += Mathf.Sin(angle * i * Mathf.Deg2Rad) * radius;
                startPos.Add(pos);
            }

            //Fire all raycasts and find the result closest to the original impact point
            float lowestDistToimpact = 1000f;
            Vector3 bestPos = impactHit.point;//If nothing could be found, use the impact point
            foreach (Vector3 start in startPos)
            {
                Vector3 offsetedStart = start;
                RaycastHit potentialDestination;
                GameObject contactMainObj;
                GameObject contactObj;

                //First shoot up
                offsetedStart.y -= 0.05f;//Just so that the plane of the hit point can be hit
                if (m_ShotIsValid(out potentialDestination, out contactMainObj, out contactObj, offsetedStart, Vector3.up, verticalOffest, castMask))
                {
                    //Here we have shot next to the impact point and hit something
                    float dist = Vector3.Distance(impactHit.point, potentialDestination.point);
                    if (dist < lowestDistToimpact)
                    {
                        bestPos = potentialDestination.point;
                    }
                }
                else
                {
                    //Then shoot down if nothing was found when shootin up
                    offsetedStart = start;
                    offsetedStart.y += _verticalOffset;
                    if (m_ShotIsValid(out potentialDestination, out contactMainObj, out contactObj, offsetedStart, Vector3.down, verticalOffest, castMask))
                    {
                        //Here we have shot next to the impact point and hit something
                        float dist = Vector3.Distance(impactHit.point, potentialDestination.point);
                        if (dist < lowestDistToimpact)
                        {
                            bestPos = potentialDestination.point;
                        }
                        //TODO create this asan out parameter
                        _foundDestinationWithDownCast = true;
                    }
                }
            }
            
            //Return the point closest to the impact, found by raycasting around the impact point
            return bestPos;
        }

        private void _SinglePullAction(float pullForce, Hookable hookableTarget, RaycastHit impactHitPoint)
        {
            //If there is nothing to pull
            if (hookableTarget == null)
            {
                m_StopHooking("There was no hookable to call the pull action on. Fast hook.");
                return;
            }

            Debugger.LogHook("Apply single pull action.");

            Sound.PlaySound(_pullActionClip, m_abilityCaster.gameObject);

            //Apply force on object
            Vector3 forceOrigin = m_GetGunTip().position;
            Vector3 attachPoint = m_GetHookAttachPoint(hookableTarget, impactHitPoint);
            Vector3 forceDirection = forceOrigin - attachPoint;

            foreach (var item in hookableTarget.GetInteractables())
            {
                item.OnInteractionWithForceStart(forceOrigin, forceDirection, pullForce, m_casterGameObject, this, null);
            }

            //Stop hook
            m_StopHooking("Because anobject has been pulled.");
        }

        #region Fast hook inputs

        void _OnJump_FastHook(ControlledInputContext context)
        {
            if (m_isHooked)
            {
                m_StopHooking("Because the player jumped.");
            }
        }

        void _OnAnchoredDown_FastHook(ControlledInputContext context)
        {

        }

        void _OnAnchoredUp_FastHook(ControlledInputContext context)
        {

        }

        void _OnSprintDown_FastHook(ControlledInputContext context)
        {

        }

        void _OnSprintUp_FastHook(ControlledInputContext context)
        {

        }

        #endregion Fast hook inputs

        #endregion Fast hook behaviour

        //**************************************************
        // Swing hook
        //**************************************************

        #region Swing hook behaviour

        private void m_StartSwingHook()
        {
            Debugger.LogHook("Swing hook has started");
            Metrics.levelData.successfulSwingHookUses += 1;
            _swingStartTime = Time.time;
            m_hookType = HookType.Swing;
            
            //If we are already in falling state, start the hook
            if (m_player.stateMachineBehaviour.activeState.IsOfType<FallingState>())
            {
                Debugger.LogHook("Player is already falling.");
                _SetupSwingProcess();
            }
            else
            {
                //Here the player is walking with the rope out, we should flag for checking the max range
                _checkRopeExtensionPastMaxRange = true;

                //Update the position of the hooked object if it is a moveable object
                if (m_hookable != null)
                {
                    _continuouslyUpdateJointAnchor = m_hookable.IsMovingObject();
                }
            }

            m_player.stateMachineBehaviour.onNewStageEntered += _CheckForStateTransition;
        }

        private void _CheckForStateTransition(BaseState newState)
        {
            if (newState.IsOfType<FallingState>())
            {
                Debugger.LogHook("Player started to fall.");
                //Here we start falling, so make sure there is a spring joint to catch us
                _SetupSwingProcess();
                return;
            }

            //If we are entering another state than falling and grappling
            if (newState.IsOfType<FallingState>() == false && newState.IsOfType<GrapplingState>() == false)
            {
                _InteruptSwingHook();

                if (newState.IsOfType<WalkingState>())
                {
                    Debugger.LogHook("Player entered walking state.");
                    //Here the player still has the hook and landed, so we should start to check for max rope length again
                    _checkRopeExtensionPastMaxRange = true;
                }
            }
        }

        /// <summary> Stop the swing functionality without retracting the rope. Used for when the player lands while swinging. </summary>
        private void _InteruptSwingHook()
        {
            float swingTravelTime = Time.time - _swingStartTime;
            Metrics.levelData.swingHookDurationTimes.Add(swingTravelTime);
            if (swingTravelTime < _validSwingHookTravelTime)
                Metrics.levelData.successfulSwingHookTravels += 1;
            
            //Remove the spring joint object
            _RemoveSpringJoint();

            _StopMaintainedPullAction();
            _StopShorteningRope();
            _StopLengtheningRope();

            m_isHooked = false;

            //Reset the collider of the character
            m_fpController.SetColliderTypeTo(ColliderType.Normal);
        }

        /// <summary> Called by the general HookStop method, this is the specific swing hook actions. </summary>
        private void _OnSwingHookStop()
        {
            float swingTravelTime = Time.time - _swingStartTime;
            Metrics.levelData.swingHookDurationTimes.Add(swingTravelTime);
            if (swingTravelTime < _validSwingHookTravelTime)
                Metrics.levelData.successfulSwingHookTravels += 1;

            //Stop updating the anchor of the spring joint
            _continuouslyUpdateJointAnchor = false;
            _checkRopeExtensionPastMaxRange = false;

            _RemoveSpringJoint();

            m_player.stateMachineBehaviour.onNewStageEntered -= _CheckForStateTransition;
        }

        void _Update_SwingHook()
        {
            if (_continuouslyUpdateJointAnchor)
            {
                _UpdateJointAnchorPosition();
                _UpdateHookProjectilePosition();
            }

            if (_checkRopeExtensionPastMaxRange)
                _CheckForRopeMaxRange(m_GetGunTip().position, m_GetHookAttachPoint(m_hookable, m_impactHitPoint), maxRange);

            if (_isShorteningRope)
                _ShortenRope();

            if (_isLengtheningRope)
                _LengthenRope();

            if (_isPullingOnRope)
            {
                _PullOnRope();
                _UpdateHookProjectilePosition();
            }
        }

        private void _CheckForRopeMaxRange(Vector3 startPos, Vector3 attachmentPos, float maxRange)
        {
            float dist = Vector3.Distance(startPos, attachmentPos);
            if (dist >= maxRange)
            {
                m_StopHooking("Because player walked past max range.");
            }
            else if (dist > _ropeMaxRangeWarningThreshold && _hasPassedMaxRangeWarning == false)
            {
                _hasPassedMaxRangeWarning = true;
                //TODO implement feedback for max rope range warning
            }
            else if (dist < _ropeMaxRangeWarningThreshold)
            {
                _hasPassedMaxRangeWarning = false;
            }
        }

        /// <summary> Creates the joint for the swing, enters the grappling state, and other setting changes. Should be called once when the swing is about to start. </summary>
        private void _SetupSwingProcess()
        {
            if (m_hookJoint == null)
                _CreateSpringJoint(m_GetGunTip().position, m_impactHitPoint, m_hookable);
            _UpdateJointAnchorPosition();
            _UpdateHookProjectilePosition();

            m_player.stateMachineBehaviour.TransitionToNewState<GrapplingState>();

            m_isHooked = true;

            //Change the collider of the character
            m_fpController.SetColliderTypeTo(ColliderType.Swing);

            //Play the sound for because we start to swing
            Sound.PlaySound(_swingingClip, m_casterGameObject);

            //We don't want to check the max range anymore, we let the joint handle it
            _checkRopeExtensionPastMaxRange = false;

            //Add a force down to increase the feeling of swing
            m_fpController.ApplyForce(Vector3.down, _startImpulseIntensity, ForceMode.Impulse);

            //If the objet we attach to is moving we start updating the joint anchor
            if (m_hookable != null)
                _continuouslyUpdateJointAnchor = m_hookable.IsMovingObject();

            if (m_hookable != null)
                _StartMaintainedPullAction(_initialPullForce, m_hookable, m_impactHitPoint);
        }

        private void _CreateSpringJoint(Vector3 startPos, RaycastHit impactHit, Hookable hookable)
        {
            m_hookJoint = m_casterGameObject.AddComponent<SpringJoint>();
            m_hookJoint.autoConfigureConnectedAnchor = false;
            m_hookJoint.spring = _spring;
            m_hookJoint.damper = _damper;
            m_hookJoint.massScale = _massScale;

            float grappleDist = Vector3.Distance(startPos, m_GetHookAttachPoint(hookable, impactHit));

            m_hookJoint.maxDistance = grappleDist;
            m_hookJoint.minDistance = grappleDist * 0.1f;
        }

        private void _UpdateJointAnchorPosition()
        {
            if (m_hookJoint == null) return;

            //Set the anchor of the spring joint to be the attachment point of the hookable object
            Vector3 attachPosition = m_GetHookAttachPoint(m_hookable, m_impactHitPoint);
            m_hookJoint.connectedAnchor = attachPosition;
        }

        private void _UpdateHookProjectilePosition()
        {
            //Set the projectile position to be the attachment point of the hookable object
            Vector3 attachPosition = m_GetHookAttachPoint(m_hookable, m_impactHitPoint);
            m_hookProjectileObject.transform.position = attachPosition;
        }

        private void _RemoveSpringJoint()
        {
            Destroy(m_hookJoint); 
            m_hookJoint = null;
        }

        #region Swing hook pull actions
        private void _StartMaintainedPullAction(float pullForce, Hookable hookableTarget, RaycastHit impactHitPoint)
        {
            Debugger.LogHook("Start applying continuous pull action.");

            Sound.PlaySound(_pullActionClip, m_abilityCaster.gameObject);

            m_hookable.OnHookableDestroyed += () => { m_StopHooking("Hookable has been destroyed."); };

            //Apply force on object
            Vector3 forceOrigin = m_GetGunTip().position;
            Vector3 attachPoint = m_GetHookAttachPoint(hookableTarget, impactHitPoint);
            Vector3 forceDirection = forceOrigin - attachPoint;
            Action interactionCancelled = () => { m_StopHooking("Interaction has been cancelled. Swing hook."); };

            foreach (var item in hookableTarget.GetInteractables())
            {
                item.OnInteractionWithForceStart(forceOrigin, forceDirection, pullForce, m_casterGameObject, this, interactionCancelled);
            }

            _isPullingOnRope = true;
        }

        void _PullOnRope()
        {
            Vector3 forceOrigin = m_GetGunTip().position;
            Vector3 attachPoint = m_GetHookAttachPoint(m_hookable, m_impactHitPoint);
            Vector3 forceDirection = forceOrigin - attachPoint;
            
            foreach (var item in m_hookable.GetInteractables())
            {
                item.InteractWithForceContinuous(forceOrigin, forceDirection, _tensionPullForce * Time.deltaTime);
            }
        }

        private void _StopMaintainedPullAction()
        {
            if (m_hookable == null) return;

            Vector3 forceOrigin = m_GetGunTip().position;
            Vector3 attachPoint = m_GetHookAttachPoint(m_hookable, m_impactHitPoint);
            Vector3 forceDirection = forceOrigin - attachPoint;

            foreach (var item in m_hookable.GetInteractables())
            {
                item.OnInteractionWithForceStop(forceDirection, 0f);
            }

            _isPullingOnRope = false;
        }
        #endregion Swing hook pull actions

        #region Swing hook inputs

        void _OnJump_SwingHook(ControlledInputContext context)
        {
            if (m_isHooked)
            {
                m_StopHooking("Because the player jumped.");
            }
        }

        void _OnAnchoredDown_SwingHook(ControlledInputContext context)
        {
            if (m_isHooked == false)
            {
                m_isAnchored = true;

                if (m_hookState == HookState.Out && m_hookable != null)
                    _StartMaintainedPullAction(_initialPullForce, m_hookable, m_impactHitPoint);
                //return;
            }

            if (m_hookType == HookType.Swing) 
            {
                if (m_hookable == null)
                    _StartShorteningRope();
                else if (m_hookable.IsMovingObject() == false)//Only shorten the rope if the hookable is not a moving object
                    _StartShorteningRope();
            }
        }

        void _OnAnchoredUp_SwingHook(ControlledInputContext context)
        {
            if (m_isAnchored)
            {
                m_isAnchored = false;
                if (_isPullingOnRope)
                    _StopMaintainedPullAction();
            }

            if (m_isHooked)
            {
                if (m_hookable == null)
                    _StopShorteningRope();
                else if (m_hookable.IsMovingObject() == false)//Only shorten the rope if the hookable is not a moving object
                    _StopShorteningRope();
                return;
            }
        }

        void _OnSprintDown_SwingHook(ControlledInputContext context)
        {
            if (m_isHooked == false)
                return;

            if (m_hookType == HookType.Swing)
            {
                if (m_hookable == null)
                    _StartLengtheningRope();
                else if (m_hookable.IsMovingObject() == false)//Only lengthen the rope if the hookable is not a moving object
                    _StartLengtheningRope();
            }
        }

        void _OnSprintUp_SwingHook(ControlledInputContext context)
        {
            if (m_isHooked)
            {
                if (m_hookable == null)
                    _StopLengtheningRope();
                else if (m_hookable.IsMovingObject() == false)//Only lengthen the rope if the hookable is not a moving object
                    _StopLengtheningRope();
                return;
            }
        }
        #endregion Swing hook inputs

        #region Controlling rope length

        private void _StartShorteningRope()
        {
            _isShorteningRope = true;
            if (m_hookJoint == null)
                _CreateSpringJoint(m_GetGunTip().position, m_impactHitPoint, m_hookable);
            _UpdateJointAnchorPosition();
            _UpdateHookProjectilePosition();
        }
        private void _ShortenRope()
        {
            m_hookJoint.maxDistance -= _ropeShorteningSpeed * Time.deltaTime;
            if (m_hookJoint.maxDistance < maxRange * 0.1f)
                m_hookJoint.maxDistance = maxRange * 0.1f;
        }
        private void _StopShorteningRope()
        {
            _isShorteningRope = false;
        }

        private void _StartLengtheningRope()
        {
            _isLengtheningRope = true;
        }
        private void _LengthenRope()
        {
            m_hookJoint.maxDistance += _ropeLengtheningSpeed * Time.deltaTime;
            if (m_hookJoint.maxDistance > maxRange)
                m_hookJoint.maxDistance = maxRange;
        }
        private void _StopLengtheningRope()
        {
            _isLengtheningRope = false;
        }

        #endregion

        #endregion Swing hook behaviour

        #endregion Specific hook behaviours

        //**************************************************
        // Utility
        //**************************************************

        #region Utility methods

        public Vector3 m_GetHookAttachPoint(Hookable hookable, RaycastHit impactHit)
        {
            if (hookable == null)
            {
                return impactHit.point;
            }

            //If we hooked a hookable object
            return hookable.GetHookAttachmentPoint();
        }

        public Transform m_GetGunTip()
        {
            if (m_abilityCaster.hookGun == null)
                Debug.LogError("Hook gun tip not assigned in the ability caster");
            return m_abilityCaster.hookGun.GetGunTip();
        }

        private bool m_ShotIsValid(out RaycastHit impactHit, out GameObject hookedMainObj, out GameObject hookedObj, Vector3 position, Vector3 direction, float range, LayerMask hookMask, bool debugAction = true)
        {
            if (debugAction)
                Debugger.LogHook("Checking if shot is valid.");

            RaycastHit hit;

            if (_castType == CastType.Raycast)
            {
                if (Physics.Raycast(position, direction, out hit, range, hookMask, QueryTriggerInteraction.Ignore))
                {
                    hookedObj = hit.collider.gameObject;
                    hookedMainObj = hit.collider.gameObject.GetMainObject();
                    impactHit = hit;

                    //If the layer of the object we hit is not correct, return false
                    int hitLayer = hit.collider.gameObject.layer;
                    if ((_validLayersMask == (_validLayersMask | (1 << hitLayer))) == false)
                        return false;

                    return true;
                }
            }
            else if (_castType == CastType.SphereCast)
            {
                if (Physics.SphereCast(position, _sphereCastRadius, direction, out hit, range, hookMask, QueryTriggerInteraction.Ignore))
                {
                    hookedObj = hit.collider.gameObject;
                    hookedMainObj = hit.collider.gameObject.GetMainObject();
                    impactHit = hit;

                    //If the layer of the object we hit is not correct, return false
                    int hitLayer = hit.collider.gameObject.layer;
                    if ((_validLayersMask == (_validLayersMask | (1 << hitLayer))) == false)
                        return false;

                    return true;
                }
            }

            //Nothing was hit so set the hit point to the max range
            hit = new RaycastHit();
            hit.point = position + direction * range;

            impactHit = hit;
            hookedMainObj = null;
            hookedObj = null;
            return false;
        }

        #endregion
    }
}
