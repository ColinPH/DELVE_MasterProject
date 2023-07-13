using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropellerCap
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Player Abilities/Interaction Ability", order = 1)]
    public class InteractionAbility : PlayerAbilityBase
    {
        public float interactionDistance = 3f;
        public LayerMask interactableMask;

        Transform _cameraTransform;
        bool _interactionIsValid = false;
        IInteractable _validInteractable;
        GameObject _lastHitMainObject;

        bool _hasBeenActivated = false;

        public override AbilityType AbilityType => AbilityType.Interaction;
        protected override void m_ActionInitialized()
        {
            base.m_ActionInitialized();

            _cameraTransform = m_abilityCaster.cameraTransform;
        }

        protected override void m_Update()
        {
            base.m_Update();

            RaycastHit hit;
            if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out hit, interactionDistance, interactableMask, QueryTriggerInteraction.Ignore))
            {
                GameObject hitObject = hit.collider.gameObject;
                GameObject potentialNewMainObject = hitObject.GetMainObject();

                if (potentialNewMainObject == _lastHitMainObject)
                    return;

                _lastHitMainObject = potentialNewMainObject;
                HUD.subtitlesControler.SetSubtitlesText("");
                _validInteractable = _lastHitMainObject.GetComponent<IInteractable>();

                if (_validInteractable == null) return;

                _hasBeenActivated = true;

                if (_validInteractable.IsInteractable(m_casterGameObject))
                {
                    _interactionIsValid = true;
                    string interactionText = _validInteractable.GetInteractionText();
                    //Show the text on screen
                    HUD.subtitlesControler.SetSubtitlesText(interactionText);
                }
            }
            else
            {
                if (_hasBeenActivated == false)
                    return;

                HUD.subtitlesControler.SetSubtitlesText("");
                _interactionIsValid = false;
                _validInteractable = null;
                _lastHitMainObject = null;
            }
        }

        protected override void m_ActionPerformed(ControlledInputContext context)
        {
            base.m_ActionPerformed(context);

            if (_interactionIsValid)
            {
                _InteractWithObject(_validInteractable, _lastHitMainObject);
                HUD.subtitlesControler.SetSubtitlesText("");
                _interactionIsValid = false;
                _validInteractable = null;
                _lastHitMainObject = null;
                return;
            }

            //This is to help debug the interaction length
            float maxLength = 10;
            RaycastHit hit;
            if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out hit, maxLength, interactableMask, QueryTriggerInteraction.Ignore))
            {
                GameObject hitObject = hit.collider.gameObject;
                float dist = Vector3.Distance(_cameraTransform.position, hit.point);
                Debug.Log("The current interaction distance is " + interactionDistance + " but the object :" + hitObject.name +
                    " is " + dist + " away.");
            }

            if (_lastHitMainObject == null) return;

            Debug.Log("There is no IInteractable interface on the object : " + _lastHitMainObject.name);
            return;
        }

        void _InteractWithObject(IInteractable interactable, GameObject hitObject)
        {
            interactable.Interact(m_casterGameObject);
        }
    }
}