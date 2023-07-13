namespace PropellerCap
{
    public enum SceneGroup
    {
        Unassigned = 0,
        Mandatory = 1,
        Level = 2,
        Tutorial = 3,
        Corridor = 3,
        UniqueScene = 4
    }

    public enum UniqueScene
    {
        Unassigned = 0,
        Intro = 1,
        MainMenu = 2,
        Credits = 3,
        HeadUpDisplay = 4,
        Hub = 5,
        Ambience = 6,
        FinalRoom = 7,
    }

    public enum Level
    {
        Unassigned = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5 = 5
    }

    public enum Tutorial
    {
        Unassigned = 0,
        Tutorial1 = 1,
        Tutorial2 = 2,
        Tutorial3 = 3,
    }

    public enum Corridor
    {
        Unassigned = 0,
        Type1 = 1,
        Type2 = 2,
        Type3 = 3
    }

    /// <summary> How to select the scenes to load within a loadable. </summary>
    public enum SceneTargets
    {
        /// <summary> Only load the main scenes. </summary>
        Main = 0,
        /// <summary> Load all scenes, both main and loadable. </summary>
        All = 1
    }

    public enum LoadingMethod
    {
        Progressive = 0,
        RandomEachTime = 1,
        RandomOnce = 2
    }
}
