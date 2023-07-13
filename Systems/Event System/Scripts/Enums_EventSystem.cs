namespace PropellerCap
{
    public enum GameEvent
    {
        Unassigned = 0,
        RunStart = 1,
        PreSceneLoad = 2,
        SceneLoaded = 3,
        PreSceneUnload = 4,
        SceneUnloaded = 5,
        SceneStart = 6,
        SceneCompleted = 7,
        SceneFailed = 8,//Not implemented
        RunFailed = 9,//Not implemented
        RunCompleted = 10,
        PlayerDied = 11,
        PlayerSpawned = 12,
        PlayerRespawned = 13,
    }

    public enum RoomEvent
    {
        Unassigned = 0,
        UpgradeAcquired,
        CurseAfflicted,
        FragmentCollected,
        AllFragmentsCollected,
        TotemAssembled
    }
}
