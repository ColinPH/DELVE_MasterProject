using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class BuildConfig
    {
        public string buildsExportFolderPath = "";
        public string scenesToBuildFolderPath = "";
        public string rootSceneName = "";
        public string executableName = "";
        public string projectCodeName = "";
        public string gameVersion = "";
        public bool launchAppAfterBuild = false;
        //Set by unity auto builder
        public string latestBuildFolderPath = "";
        public string latestBuildExecutablePath = "";
        public string buildTime = "";
        public int buildSize = 0;
    }
}