
namespace PropellerCap
{
    public static class Metrics
    {
        public static MetricsManager manager { get; set; }


        public static PlayerMetrics player { get; set; }
        public static GameMetrics game { get; set; }

        #region Player data collection
        public static PlayerSessionData sessionData
        {
            get
            {
                if (manager.sessionData == null)
                {
                    Debugger.LogError($"No '{nameof(PlayerSessionData)}' has been open, opening it now.");
                    manager.OpenSessionData();
                }
                return manager.sessionData;
            }
        }
        public static PlayerRunData runData
        {
            get
            {
                if (manager.sessionData == null)
                {
                    Debugger.LogError($"No '{nameof(PlayerRunData)}' has been open, opening it now.");
                    manager.OpenRunData(Managers.runManager.ActiveRunObject.RunName);
                }
                return manager.runData;
            }
        }
        public static PlayerLevelData levelData
        {
            get
            {
                if (manager.sessionData == null)
                {
                    Debugger.LogError($"No '{nameof(PlayerLevelData)}' has been open, opening it now.");
                    manager.OpenLevelData("Level Data not open");
                }
                return manager.levelData;
            }
        }


        #endregion Player data collection

        //Put this somewhere else one day...
        public delegate void CoinCollectedHandler();
        public static CoinCollectedHandler OnCoinCollected { get; set; }
    }
}
