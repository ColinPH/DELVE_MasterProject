using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    [CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Settings/Quality Of Life Settings", order = 1)]
    public class QOLSettings : ScriptableObject
    {
        //[Header("")]
        public GeneralQOLSettings generalQOLSettings = new GeneralQOLSettings();
        [Header("Naming Settings")]
        public NamingConventionsSettings namingConventionsSettings = new NamingConventionsSettings();
        [Header("Label Settings")]
        public LabelsSettings labelSettings = new LabelsSettings();
        [Header("Texture and Material Settings")]
        public TextureToMaterialSettings textureToMaterialSettings = new TextureToMaterialSettings();
        [Header("Asset Folder Template Settings")]
        public AssetFoldersTemplateSettings assetFoldersTemplateSettings = new AssetFoldersTemplateSettings();
        [Header("Prefab Creation Settings")]
        public PrefabCreationSettings prefabCreationSettings = new PrefabCreationSettings();
        [Header("Automated Screenshot Settings")]
        public AutomatedScreenshotSetings automatedScreenshotSettings = new AutomatedScreenshotSetings();

        private void OnEnable()
        {
            generalQOLSettings.OnContainerEnable();
            namingConventionsSettings.OnContainerEnable();
            labelSettings.OnContainerEnable();
            textureToMaterialSettings.OnContainerEnable();
            assetFoldersTemplateSettings.OnContainerEnable();
            prefabCreationSettings.OnContainerEnable();
            automatedScreenshotSettings.OnContainerEnable();
        }

        private void Reset()
        {
            generalQOLSettings.OnContainerEnable();
            namingConventionsSettings.OnContainerEnable();
            labelSettings.OnContainerEnable();
            textureToMaterialSettings.OnContainerEnable();
            assetFoldersTemplateSettings.OnContainerEnable();
            prefabCreationSettings.OnContainerEnable();
            automatedScreenshotSettings.OnContainerEnable();
        }
    }
}