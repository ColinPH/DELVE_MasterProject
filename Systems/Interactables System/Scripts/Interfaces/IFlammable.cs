using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public interface IFlammable
    {
        LightType GetLightType();
        bool IsIgnited();

        //Flammable ignite
        void Ignite(LightType lightType);
        public delegate void IgniteHandler();
        public void RegisterToIgnite(IgniteHandler igniteDelegate);
        public void DeregisterFromIgnite(IgniteHandler igniteDelegate);


        //Flammable extinguish
        void Extinguish();
        public delegate void ExtinguishHandler();
        public void RegisterToExtinguish(ExtinguishHandler extinguishDelegate);
        public void DeregisterFromExtinguish(ExtinguishHandler extinguishDelegate);

        //Flammable light type change
        public delegate void LightTypeChangeHandler(LightType newLightType);
        public void RegisterToLightTypeChange(LightTypeChangeHandler lightTypeChangeDelegate);
        public void DeregisterFromLightTypeChange(LightTypeChangeHandler lightTypeChangeDelegate);
    }
}
