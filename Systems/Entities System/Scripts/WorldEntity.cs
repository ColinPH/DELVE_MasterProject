using PropellerCap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEntity : WorldObject, IDamageable
{
    [Tooltip("Some tooltip for testing")]
    public string entityName = "Default Name";

    //General entity events
    public delegate void OnEntityDeath();
    public OnEntityDeath onEntityDeath;

    //Events for the IDamageable interface
    public delegate void OnDamageTaken(Damages damages);
    public OnDamageTaken onDamagesTaken { get; set; }
    public delegate HealthInfo OnHealthInfoRequest();
    public OnHealthInfoRequest onHealthInfoRequest { get; set; }


    #region IDamageable interface

    public void IDealDamage(Damages damages)
    {
        onDamagesTaken?.Invoke(damages);
    }

    public HealthInfo IGetHealthInfo()
    {
        if (onHealthInfoRequest != null)
            return onHealthInfoRequest();
        else
        {
            Debug.LogWarning(
                "Trying to access "
                + typeof(HealthInfo) + " on "
                + gameObject.name
                + " but none of the components returns a value.");
            return new HealthInfo();
        }
    }

    #endregion


    public void InvokeOnEntityDeath()
    {
        onEntityDeath?.Invoke();
    }
}