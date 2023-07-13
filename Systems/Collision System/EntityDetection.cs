using PropellerCap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDetection : DetectionParent
{
    //Triggers
    [SerializeField] string _agroTriggerDetectionName = "Agro";
    [SerializeField] string _attackTriggerDetectionName = "Attack";
    public DetectionTriggerEvent onAgroTriggerEvent;
    public DetectionTriggerEvent onAttackTriggerEvent;

    //Collisions
    [SerializeField] string _headCollisionDetectionName = "Head";
    [SerializeField] string _bodyCollisionDetectionName = "Body";
    public DetectionCollisionEvent onHeadCollisionEvent;
    public DetectionCollisionEvent onBodyCollisionEvent;

    /*protected override void m_EventsAssignments()
    {
        base.m_EventsAssignments();

        onAgroTriggerEvent = m_GetNewTriggerEvent(_agroTriggerDetectionName);

        onAttackTriggerEvent = m_GetNewTriggerEvent(_attackTriggerDetectionName);





        onHeadCollisionEvent = m_GetNewCollisionEvent(_headCollisionDetectionName);
        
        onBodyCollisionEvent = m_GetNewCollisionEvent(_bodyCollisionDetectionName);
    }*/
}
