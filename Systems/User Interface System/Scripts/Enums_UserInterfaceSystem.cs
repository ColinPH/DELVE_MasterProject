
namespace PropellerCap
{
    public enum CustomPassType
    {
        Unassigned = 0,
        SpeedEffect = 1,
        InsanityEffect = 2,
        HealingEffect = 3,
    }

    public enum RenderContainerType
    {
        Unassigned = 0,
        overlayBackground = 1,
        overlayActive = 2,
        overlayForeground = 3,
        cameraSpaceBackground = 4,
        cameraSpaceActive = 5,
        cameraSpaceForeground = 6,
    }

    public enum CustomPassLoadMode
    {
        /// <summary> Keeps the other custom passes. </summary>
        Additive = 0,
        /// <summary> Removes the other custom passes. </summary>
        Single = 1
    }
}
