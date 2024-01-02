using System;
using System.Collections.Generic;
using System.IO;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using UnityEngine;

namespace StarSquad.Lobby.UI.Screens.Progression
{
    public class StaticLeagueManager
    {
        private static string GetProgressionPath()
        {
#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                return Application.dataPath + "/Editor/TestData/progression.json";
            }
#endif
            return AssetManager.Get().GetFile("progression.json");
        }

        public List<ProgressionData> progression;
        
        public string[] tiers;

        [Serializable]
        public class ProgressionList
        {
            public List<ProgressionData> progression;
        }

        [Serializable]
        public class ProgressionReward
        {
            public int type;
            public int amount;
        }

        [Serializable]
        public class ProgressionLeague
        {
            public int league;
            public int tier;
        }

        [Serializable]
        public class ProgressionData
        {
            public int id;
            public int trophies;
            public ProgressionReward reward;
            public bool hasLeague;
            public ProgressionLeague league;
        }


        public void Load()
        {
            this.tiers = new[]
            {
                "V",
                "IV",
                "III",
                "II",
                "I"
            };

            var line = File.ReadAllText(GetProgressionPath());

            var json = JsonUtility.FromJson<ProgressionList>(line);

            this.progression = json.progression;
        }
    }
}