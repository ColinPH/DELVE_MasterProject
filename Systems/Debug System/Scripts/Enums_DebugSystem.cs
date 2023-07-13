namespace PropellerCap
{
    public enum DebugType
    {
        /// <summary> Default log message, will always be shown in logs. </summary>
        Unassigned,
        /// <summary> Initialization of the game. Each manager has Init, Awake, Start etc... </summary>
        Initialization,
        /// <summary>  Actions performed by the local level managers. Each action should log what it does. </summary>
        LocalManagerAction,
        /// <summary> For errors, will always be shown in logs. </summary>
        Error,
        /// <summary> For warnings, will always be shown in logs. </summary>
        Warning,
        /// <summary> When scenes are loaded /unloaded. Also shows the time it took to load the different scenes/levels. </summary>
        SceneLoading,
        /// <summary> All the logic of the hook ability. </summary>
        Hook,
        /// <summary> All the logic of the flare ability. </summary>
        Flare,
        /// <summary> For all the events happening in the game. </summary>
        Event,
        /// <summary> For all the states of the player. </summary>
        State, 
        /// <summary> All teh custom behaviours such as MyAwake, MyStart, MyUpdate etc... </summary>
        MyBehaviour,
    }
}
