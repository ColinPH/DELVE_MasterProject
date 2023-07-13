using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    public class ApplyTextureToMaterialOption
    {
        [MenuItem("Assets/Propeller Cap/Apply Texture to Material", false, 19)]
        public static void AssignSelectedTexturesToMaterial()
        {
            //Separate the textures from the materials
            List<Texture> textures = new List<Texture>();
            List<Material> materials = new List<Material>();

            foreach (Object item in Selection.objects)
            {
                if (item is Material) materials.Add((Material)item);
                else if (item is Texture) textures.Add((Texture)item);
            }

            //Make sure the changes can be undone
            Undo.RecordObjects(materials.ToArray(), "Applied textures to material through context menu.");

            //Apply the textures to all materials
            foreach (Material mat in materials)
            {
                foreach (Texture tex in textures)
                {
                    TextureTypes targetType = Utils.GetTextureTypeFromFileName(tex.name);

                    mat.SetTexture(Utils.GetDefaultMaterialShaderTextureName(targetType), tex);
                }
            }
            
            //Make sure the changes are saved
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Propeller Cap/Apply Texture to Material", true, 19)]
        public static bool CheckSelection()
        {
            bool selectionHasMaterial = false;
            bool selectionHasTexture = false;

            foreach (Object item in Selection.objects)
            {
                if (item is Material) selectionHasMaterial = true;
                if (item is Texture) selectionHasTexture = true;
            }
            
            return selectionHasMaterial && selectionHasTexture;
        }
    }
}