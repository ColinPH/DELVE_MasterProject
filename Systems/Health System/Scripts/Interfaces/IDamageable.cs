using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public interface IDamageable
    {
        public void IDealDamage(Damages damages);
        public HealthInfo IGetHealthInfo();
    }
}