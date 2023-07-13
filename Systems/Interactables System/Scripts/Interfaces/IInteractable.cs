using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public interface IInteractable
    {
        /// <summary> Default way to interact with the object. No condition applied to the interaction. </summary>
        void Interact(GameObject callingEntity);
        /// <summary> Called once when the interaction has started. </summary>
        void OnInteractionStart(Action onInteractionCancelled);
        /// <summary> Called once when the interaction has been stopped. </summary>
        void OnInteractionStop();
        /// <summary> Interact with the object by applying a force each frame. Should already been multiplied by Time.Deltatime. </summary>
        void InteractWithForceContinuous(Vector3 forceOrigin, Vector3 direction, float intensity);
        /// <summary> Called once at the start of the interaction with force. </summary>
        void OnInteractionWithForceStart(Vector3 forceOrigin, Vector3 direction, float intensity, GameObject pullingObject, object caller, Action onInteractionCancelled);
        /// <summary> Called once at the end of the interaction with force. </summary>
        void OnInteractionWithForceStop(Vector3 direction, float intensity);
        /// <summary> Highlight the object to make it more visible for the player. </summary>
        void Highlight();
        /// <summary> Returns whether it is possible to interact with the object. </summary>
        /// <param name="initiatorObject">The GameObject which tries to interact with the interactable object.</param>
        bool IsInteractable(GameObject initiatorObject);
        /// <summary> Text which describes what needs to be done to interact with the object. </summary>
        string GetInteractionText();
    }
}