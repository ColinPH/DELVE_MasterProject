using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class HUDComponent : MonoBehaviour
    {
        public bool hideOnInitialization = false;
        public RenderContainerType containerType = RenderContainerType.Unassigned;

        protected RectTransform m_rectTransform;

        private void Start()
        {
            m_rectTransform = GetComponent<RectTransform>();
            MonoStart();
        }
        protected virtual void MonoStart()
        {

        }
        public virtual void OnComponentInstantiation()
        {

        }

        public virtual void ShowComponent()
        {
            
        }

        public virtual void HideComponent()
        {
            
        }

        public void HideOnInitialization()
        {
            if (hideOnInitialization)
                HideComponent();
        }

        /// <summary> Called before the UI manager calls Destroy() on the component's gameobject. </summary>
        public virtual void OnComponentDestruction()
        {
            
        }
    }
}