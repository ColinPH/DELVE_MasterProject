using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap.QA
{
    public class GameVersionData
    {
        public int publicRelease = 0;
        public int internalRelease = 0;
        public int testBuild = 0;

        public GameVersionData(int publicRelease, int internalRelease, int testBuild)
        {
            this.publicRelease = publicRelease;
            this.internalRelease = internalRelease;
            this.testBuild = testBuild;
        }
        public GameVersionData(string version)
        {
            string[] words = version.Split('.');

            if (words.Length != 3)
            {
                Debug.LogError("There is a problem with the naming of the version of the game, check the player settings.");
                publicRelease = -1;
                internalRelease = -1;
                testBuild = -1;
                return;
            }

            if (int.TryParse(words[0], out publicRelease) == false)
                Debug.Log("Could not parse the version of the game, check the player settings and follow the naming convention");
            if (int.TryParse(words[1], out internalRelease) == false)
                Debug.Log("Could not parse the version of the game, check the player settings and follow the naming convention");
            if (int.TryParse(words[2], out testBuild) == false)
                Debug.Log("Could not parse the version of the game, check the player settings and follow the naming convention");
        }

        public static implicit operator string(GameVersionData gvd)
        {
            return gvd.publicRelease + "." + gvd.internalRelease + "." + gvd.testBuild;
        }
    }
}
