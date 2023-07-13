using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PropellerCap.EditorUsability
{
    [Serializable]
    public class TextureToMaterialSettings
    {
        [Tooltip("The shader used by the newly created material when converting textures to a material.")]
        public Shader defaultMaterialShader = null;

        /// <summary>
        /// Called when the scriptable object that contains these settings is Enabled
        /// </summary>
        public void OnContainerEnable()
        {
            if (defaultMaterialShader == null)
                defaultMaterialShader = Shader.Find("Standard");
        }
    }
}