using PropellerCap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpecificScript : MonoBehaviour
{
    public string onHeadContact = "example";
    private void Start()
    {
        GetComponent<EntityDetection>().onAgroTriggerEvent.enter += OnAgroZoneEnter;
        //GetComponent<EntityDetection>().onAgroTriggerEvent.stay += OnAgroZoneStay;
        GetComponent<EntityDetection>().onAgroTriggerEvent.exit += OnAgroZoneExit;

        GetComponent<EntityDetection>().onAttackTriggerEvent.enter += OnAttackZoneEnter;
        //GetComponent<EntityDetection>().onAttackTriggerEvent.stay += OnAttackZoneStay;
        GetComponent<EntityDetection>().onAttackTriggerEvent.exit += OnAttackZoneExit;

        GetComponent<EntityDetection>().onHeadCollisionEvent.enter += OnHeadCollisionEnter;
        GetComponent<EntityDetection>().onBodyCollisionEvent.enter += OnBodyCollisionEnter;
    }

    private void OnHeadCollisionEnter(Collision other)
    {
        string objName = other.GetMainObject().name;
        Debug.Log(onHeadContact + objName);
    }

    private void OnBodyCollisionEnter(Collision other)
    {
        string objName = other.GetMainObject().name;
        Debug.Log("Body has been hit by " + objName);
    }


    private void OnAgroZoneEnter(Collider other)
    {
        string objName = other.GetMainObject().name;
        Debug.Log(objName + " has entered the agro zone");
    }
    private void OnAgroZoneStay(Collider other)
    {
        string objName = other.GetMainObject().name;
        Debug.Log(objName + " is in the agro zone");
    }
    private void OnAgroZoneExit(Collider other)
    {
        string objName = other.GetMainObject().name;
        Debug.Log(objName + " has exited the agro zone");
    }

    private void OnAttackZoneEnter(Collider other)
    {
        string objName = other.GetMainObject().name;
        Debug.Log(objName + " has entered the Attack zone");
    }
    private void OnAttackZoneStay(Collider other)
    {
        string objName = other.GetMainObject().name;
        Debug.Log(objName + " is in the Attack zone");
    }
    private void OnAttackZoneExit(Collider other)
    {
        string objName = other.GetMainObject().name;
        Debug.Log(objName + " has exited the Attack zone");
    }
}
