using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PropellerCap
{
    [AddComponentMenu("AAPropellerCap/Interactable System/Activation Targets/" + nameof(Door))]
    public class Door : ActivationTarget
    {
        enum DoorState { Unassigned, Opening, Closing, Opened, Closed }
        enum DoorStartState { Opened, Closed }

        [SerializeField] DoorStartState _startState = DoorStartState.Closed;
        [SerializeField] bool _singleCycle = false;
        [SerializeField] bool _onlyOpens = false;
        [SerializeField] bool _onlyCloses = false;
        [Header("Animation Settings")]
        [SerializeField] GameObject _animatedBody;
        [SerializeField] float _animationOpenTime = 3f;
        [SerializeField] float _animationCloseTime = 3f;
        [SerializeField] float _animOpenSpeedMultiplier = 1f;
        [SerializeField] string _openSpeedParamName = "OpenSpeedFactor";
        [SerializeField] float _animCloseSpeedMultiplier = 1f;
        [SerializeField] string _closeSpeedParamName = "CloseSpeedFactor";
        [SerializeField] string _animTriggerOpen = "Open";
        [SerializeField] string _animTriggerClose = "Close";
        [Header("Sounds")]
        [SerializeField] SoundClip _doorOpenSound;
        [SerializeField] SoundClip _doorOpenEndSound;
        [SerializeField] SoundClip _doorCloseFallingSound;
        [SerializeField] SoundClip _doorCloseImpactSound;

        bool _doorIsMoving = false;
        Animator _animator;
        float _animationEndTime = 0f;

        bool _doorCycleComplete = false;
        bool _hasBeenDeactivated = false;
        bool _hasBeenActivated = false;
        DoorState _doorState = DoorState.Unassigned;

        public override string worldName => nameof(Door);
        protected override void MyAwake()
        {
            base.MyAwake();
            _animator = m_FetchForComponent<Animator>(_animatedBody);
        }

        protected override void MyStart()
        {
            base.MyStart();

            if (_startState == DoorStartState.Opened)
                Activate();
            else if (_startState == DoorStartState.Closed)
            {
                _doorState = DoorState.Closed;
            }
        }

        protected override void MyUpdate()
        {
            if (_doorIsMoving == false)
                return;

            if (Time.realtimeSinceStartup >= _animationEndTime)
            {
                _doorIsMoving = false;
                if (_doorState == DoorState.Opening)
                    _DoorOpened();
                else if (_doorState == DoorState.Closing)
                    _DoorClosed();
            }
        }

        #region ActivationTarget overrides
        /// <summary> Open the door. </summary>
        protected override void m_Activate()
        {
            if (_doorCycleComplete && _singleCycle || _onlyCloses || _doorState == DoorState.Opened)
                return;

            _hasBeenActivated = true;
            _hasBeenDeactivated = false;

            if (_doorIsMoving)
                return;

            //Open the door
            Sound.PlaySound(_doorOpenSound, gameObject);
            _animator.SetTrigger(_animTriggerOpen);
            _animator.SetFloat(_openSpeedParamName, _animOpenSpeedMultiplier);
            _animationEndTime = Time.realtimeSinceStartup + _animationOpenTime;
            _doorIsMoving = true; 
            _doorState = DoorState.Opening;
        }

        /// <summary> Close the door. </summary>
        protected override void m_Deactivate()
        {
            if (_doorCycleComplete && _singleCycle || _onlyOpens || _doorState == DoorState.Closed)
                return;

            _hasBeenActivated = false;
            _hasBeenDeactivated = true;

            if (_doorIsMoving)
                return;

            //Close the door
            Sound.PlaySound(_doorCloseFallingSound, gameObject);
            _animator.SetTrigger(_animTriggerClose);
            _animator.SetFloat(_closeSpeedParamName, _animCloseSpeedMultiplier);
            _animationEndTime = Time.realtimeSinceStartup + _animationCloseTime;
            _doorIsMoving = true;
            _doorState = DoorState.Closing;
        }

        #endregion

        void _DoorOpened()
        {
            //Sound for the door reaching fully opened 
            Sound.PlaySound(_doorOpenEndSound, gameObject);
            InvokeActivationComplete();

            _doorState = DoorState.Opened;
            _hasBeenActivated = false;

            if (_hasBeenDeactivated)
                Deactivate();
        }

        void _DoorClosed()
        {
            _doorCycleComplete = true;
            //Sound for the door impacting the ground
            Sound.PlaySound(_doorCloseImpactSound, gameObject);
            InvokeDeactivationComplete();
            
            _doorState = DoorState.Closed;
            _hasBeenDeactivated = false;

            if (_hasBeenActivated)
                Activate();
        }
    }
}