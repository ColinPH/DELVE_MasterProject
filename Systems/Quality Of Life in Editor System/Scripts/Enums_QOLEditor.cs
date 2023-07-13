
namespace PropellerCap.EditorUsability
{
    /// <summary>
    /// The types of textures that are used to populate a material, Albedo, Normal etc..
    /// </summary>
    public enum TextureTypes
    {
        NonExistant = 0,
        Albedo = 1,
        Metallic = 2,
        Normal = 3,
        Height = 4,
        Occlusion = 5,
        Emission = 6,
        DetailMask = 7
    }

    /// <summary>
    /// The different types of assets that are used in the project, used in the naming convention.
    /// </summary>
    public enum AssetType
    {
        NonExistant = 0,
        SceneAsset = 1,
        PrefabAsset = 2,
        MaterialAsset = 3,
        TextureAsset = 4,
        FBXAsset = 5,
        VisualEffectAsset = 6,
        SoundEffectAsset = 7,
        ShaderGraphAsset = 8,
        SubShaderGraphAsset = 9,
        SpriteAsset = 10,
        FontAsset = 11,
        MusicAsset = 12,
        TimelineAsset = 13,
        AnimationAsset = 14,
        AnimationControllerAsset = 15
    }
}