using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    public class CreateMaterialFromTextureOption
    {
        [MenuItem("Assets/Propeller Cap/Convert Texture to Material", false, 19)]
        public static void AssignSelectedTexturesToMaterial()
        {
            Debug.Log("Convert the selected textures into a material");

            List<Texture> selectedTextures = new List<Texture>();
            foreach (Object texture in Selection.objects)
            {
                selectedTextures.Add((Texture)texture);
            }

            Texture mainTexture = (Texture)Selection.objects[0];
            string texFolderPath = Utils.GetFileParentFolderFullPath(mainTexture);
            texFolderPath = Utils.GetPathRelativeToProjectFolder(texFolderPath);
            string matFileName = Utils.ChangeAssetFileNamePrefix(mainTexture.name, AssetType.MaterialAsset);
            NamingInfo nameInfo = new NamingInfo(matFileName);
            matFileName = nameInfo.GetNameWithoutSuffix();
            
            string matFilePath = texFolderPath + "\\" + matFileName + ".mat";

            //Create a new material
            TextureToMaterialSettings settings = Utils.FindQOLSettings().textureToMaterialSettings;
            Material mat = new Material(settings.defaultMaterialShader);

            //Assign the selected textures to the newly created material
            foreach (Texture tex in selectedTextures)
            {
                TextureTypes targetType = Utils.GetTextureTypeFromFileName(tex.name);
                mat.SetTexture(Utils.GetDefaultMaterialShaderTextureName(targetType), tex);
            }

            //TODO Make sure the file name is unique
            

            if (false)
            {
                //If we are in a folder called Textures, we create the material in the folder called Materials

            }
            else
            {
                //If we are not in a Textures folder we create the material in the same folder as the texture
                AssetDatabase.CreateAsset(mat, matFilePath);
            }
        }

        [MenuItem("Assets/Propeller Cap/Convert Texture to Material", true, 19)]
        public static bool CheckSelection()
        {
            bool selectionIsOnlyTextures = true;

            foreach (Object item in Selection.objects)
            {
                if (item is not Texture)
                    selectionIsOnlyTextures = false;
            }

            return selectionIsOnlyTextures;
        }
    }
}