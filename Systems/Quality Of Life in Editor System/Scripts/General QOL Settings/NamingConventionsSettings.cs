using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PropellerCap.EditorUsability
{
    [Serializable]
    public class NamingConventionsSettings
    {
        public string rootSceneName = "Scene_Root";
        public string textSplitter = "_";
        [Header("Assets prefixes")]
        public string _sceneAssets = "Scene";
        public string _prefabAssets = "Prefab";
        public string _materialAssets = "Mat";
        public string _textureAssets = "Tex";
        public string _FBXAssets = "FPX";
        public string _visualEffectsAssets = "VFX";
        public string _soundEffectsAssets = "SFX";
        public string _shaderGraphAssets = "Shader";
        public string _subShaderGraphAssets = "SubShader";
        public string _spriteAssets = "Sprite";
        public string _fontAssets = "Font";
        public string _musicAssets = "Music";
        public string _timelineAssets = "Timeline";
        public string _animationAssets = "Anim";
        public string _animationControllerAssets = "AnimCtrl";
        [Header("Texture suffixes")]
        public string _albedoSuffix = "A";
        public string _metallicMapSuffix = "M";
        public string _normalMapSuffix = "N";
        public string _heightMapSuffix = "H";
        public string _occlusionSuffix = "O";
        public string _emissionSuffix = "L";
        public string _detailMaskSuffix = "Mask";

        /// <summary>
        /// Called when the scriptable object that contains these settings is Enabled
        /// </summary>
        public void OnContainerEnable()
        {

        }

        public bool IsSuffix(string text)
        {
            return (
                text == _albedoSuffix
                || text == _metallicMapSuffix
                || text == _normalMapSuffix
                || text == _heightMapSuffix
                || text == _occlusionSuffix
                || text == _emissionSuffix
                || text == _detailMaskSuffix
                );
        }

        public string GetTextureSuffixFromType(TextureTypes texType)
        {
            switch (texType)
            {
                case TextureTypes.NonExistant:
                    return "Error";
                case TextureTypes.Albedo:
                    return _albedoSuffix;
                case TextureTypes.Metallic:
                    return _metallicMapSuffix;
                case TextureTypes.Normal:
                    return _normalMapSuffix;
                case TextureTypes.Height:
                    return _heightMapSuffix;
                case TextureTypes.Occlusion:
                    return _occlusionSuffix;
                case TextureTypes.Emission:
                    return _emissionSuffix;
                case TextureTypes.DetailMask:
                    return _detailMaskSuffix;
                default:
                    return "MissingType";
            }
        }

        public TextureTypes GetTextureTypeFromSuffix(string suffix)
        {
            if (suffix == _albedoSuffix)
                return TextureTypes.Albedo;
            else if (suffix == _metallicMapSuffix)
                return TextureTypes.Metallic;
            else if (suffix == _normalMapSuffix)
                return TextureTypes.Normal;
            else if (suffix == _heightMapSuffix)
                return TextureTypes.Height;
            else if (suffix == _occlusionSuffix)
                return TextureTypes.Occlusion;
            else if (suffix == _emissionSuffix)
                return TextureTypes.Emission;
            else if (suffix == _detailMaskSuffix)
                return TextureTypes.DetailMask;
            else
            {
                Debug.LogWarning("The suffix \"" + suffix + "\" seems to be wrong. Check the file name or the " + typeof(QOLSettings) + " asset.");
                return TextureTypes.NonExistant;
            }
        }

        public string GetAssetPrefixFromType(AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.NonExistant:
                    return "Error";
                case AssetType.SceneAsset:
                    return _sceneAssets;
                case AssetType.PrefabAsset:
                    return _prefabAssets;
                case AssetType.MaterialAsset:
                    return _materialAssets;
                case AssetType.TextureAsset:
                    return _textureAssets;
                case AssetType.FBXAsset:
                    return _FBXAssets;
                case AssetType.VisualEffectAsset:
                    return _visualEffectsAssets;
                case AssetType.SoundEffectAsset:
                    return _soundEffectsAssets;
                case AssetType.ShaderGraphAsset:
                    return _shaderGraphAssets;
                case AssetType.SubShaderGraphAsset:
                    return _subShaderGraphAssets;
                case AssetType.SpriteAsset:
                    return _spriteAssets;
                case AssetType.FontAsset:
                    return _fontAssets;
                case AssetType.MusicAsset:
                    return _musicAssets;
                case AssetType.TimelineAsset:
                    return _timelineAssets;
                case AssetType.AnimationAsset:
                    return _animationAssets;
                case AssetType.AnimationControllerAsset:
                    return _animationControllerAssets;
                default:
                    return "MissingType";
            }
        }
    
        public AssetType GetAssetTypeFromPrefix(string prefix)
        {
            if (prefix == _sceneAssets)
                return AssetType.SceneAsset;
            else if (prefix == _prefabAssets)
                return AssetType.PrefabAsset;
            else if (prefix == _materialAssets)
                return AssetType.MaterialAsset;
            else if (prefix == _textureAssets)
                return AssetType.TextureAsset;
            else if (prefix == _FBXAssets)
                return AssetType.FBXAsset;
            else if (prefix == _visualEffectsAssets)
                return AssetType.VisualEffectAsset;
            else if (prefix == _soundEffectsAssets)
                return AssetType.SoundEffectAsset;
            else if (prefix == _shaderGraphAssets)
                return AssetType.ShaderGraphAsset;
            else if (prefix == _subShaderGraphAssets)
                return AssetType.SubShaderGraphAsset;
            else if (prefix == _spriteAssets)
                return AssetType.SpriteAsset;
            else if (prefix == _fontAssets)
                return AssetType.FontAsset;
            else if (prefix == _musicAssets)
                return AssetType.MusicAsset;
            else if (prefix == _timelineAssets)
                return AssetType.TimelineAsset;
            else if (prefix == _animationAssets)
                return AssetType.AnimationAsset;
            else if (prefix == _animationControllerAssets)
                return AssetType.AnimationControllerAsset;
            else
                return AssetType.NonExistant;
        }
    }
}