using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class FlareGun : MonoBehaviour
    {
        [SerializeField] Transform _gunTip;
        [SerializeField] LightType _lightType = LightType.Crystal;



        public LightType lightType => _lightType;

        public void ProjectileShot(GameObject projectile)
        {
            projectile.GetComponent<Flare>().SetLightType(_lightType);
        }

        public Vector3 GetProjectileInstantiationPosition()
        {
            return _gunTip.position;
        }

    }
}
