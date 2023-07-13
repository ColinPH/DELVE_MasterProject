public class README_Collision_System
{
    /*
    
                    GENERAL INFO

    In this system, the root object has a component __DetectionParent__ which gets triggered by
    the other child components placed on the nested objects that have Collider components.

    To access the events from the __DetectionParent__, create a custom class that inherits from it
    as shown in the example bellow. Then other classes can register to those events.

    When using OnCollision or OnTrigger effects, the children call the parent's events using a
    string which is set in the child component. That string needs to be matched in the parent's code.

    A __DetectionParent__ can have both nested __CollisionDetectionChild__ and nested __TriggerDetectionChild__

    Rigidbodies are required only on the object that has the component __DetectionParent__, other object 
    with Colliders that do not have Rigidbodies will still activate the events from the object that has
    both a __DetectionParent__ and a __Rigidbody__

    Moving objects with a __DetectionParent__, parent events won't be fired unless that object has a __Rigidbody__



                    NEW EXTENSIONS

    GameObject.GetMainObject()

    Returns the GameObject of the __DetectionParent__ of the triggered/collided child



                    COLLISIONS

    SETUP - Nested Colliders with NO OnCollision effects on parent object

    1_Create new object
    2_Add the component __DetectionParent__
    3_On the nested objects that have a Collider, add the component __CollisionDetectionChild__

    SETUP - Nested Colliders WITH OnCollision effects on parent object

    1_Create new Object
    2_Add a __Rigidbody__ component to the parent object
    2_Add the component __EntityDetection__ to the parent object
    3_On the nested objects that have a Collider, add the component __CollisionDetectionChild__



                    TRIGGERS

    SETUP - Nested Triggers with NO OnTrigger effects on parent object

    1_Create new object
    2_Add the component __DetectionParent__
    3_On the nested objects that have a Trigger, add the component __TriggerDetectionChild__

    SETUP - Nested Triggers WITH OnTrigger effects on parent object

    1_Create new Object
    2_Add a __Rigidbody__ component to the parent object
    2_Add the component __EntityDetection__ to the parent object
    3_On the nested objects that have a Trigger, add the component __TriggerDetectionChild__



                    EXAMPLE

    //********************************************************************
    //* Copy the examples for trigger and collision to add custom events *
    //*                                                                  *
    //*  /!\ OnCollisionExit will be called on all registered events /!\ *
    //*                                                                  *
    //********************************************************************

    //The names are used to match with the different triggers of the gameObject
    //Here is an example code for a child class that would inherit from DetectionParent


    [Header("Trigger detections")]
    [SerializeField] string _exampleTriggerDetectionName = "Example";
    public DetectionTriggerEvent onExampleTriggerEvent;

    [Header("Collision detections")]
    [SerializeField] string _exampleCollisionDetectionName = "Example";
    public DetectionCollisionEvent onExampleCollisionEvent;

    protected override void m_EventsAssignments()
    {
        base.m_EventsAssignments();

        onExampleTriggerEvent = m_GetNewTriggerEvent(_exampleTriggerDetectionName);

        onExampleCollisionEvent = m_GetNewCollisionEvent(_exampleCollisionDetectionName);
    }


    */
}
