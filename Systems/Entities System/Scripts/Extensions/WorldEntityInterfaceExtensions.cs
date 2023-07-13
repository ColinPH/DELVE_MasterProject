using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropellerCap
{
    public static class WorldEntityInterfaceExtensions
    {
        #region IDamageable interface

        public static IDamageable IDamageable(this GameObject obj)
        {
            IDamageable damageableInterface = obj.GetComponent<IDamageable>();

            if (damageableInterface == null)
            {
                Debug.LogError(
                    "None of the components on "
                    + obj.name
                    + " implement the "
                    + typeof(IDamageable)
                    + " interface.");
            }

            return damageableInterface;
        }

        public static IDamageable IDamageable(this WorldEntity worldEntity)
        {
            return worldEntity.gameObject.IDamageable();
        }

        #endregion
    }
}