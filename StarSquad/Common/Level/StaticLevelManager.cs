using System;
using System.Collections.Generic;
using System.IO;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using UnityEngine;

namespace StarSquad.Common.Level
{
    public class StaticLevelManager
    {
        public List<LevelData> levels;

        private static string GetHeroesPath()
        {
#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                return Application.dataPath + "/Editor/TestData/level.json";
            }
#endif
            return AssetManager.Get().GetFile("level.json");
        }

        [Serializable]
        public class LevelList
        {
            public List<LevelData> levels;
        }

        [Serializable]
        public class LevelData
        {
            public int points;
            public LevelReward reward;
        }

        [Serializable]
        public class LevelReward
        {
            public int type;
            public int amount;
        }

        public void Load()
        {
            var line = File.ReadAllText(GetHeroesPath());

            var json = JsonUtility.FromJson<LevelList>(line);

            this.levels = json.levels;
        }

#if UNITY_EDITOR
        private static StaticLevelManager instance;
#endif

        public static StaticLevelManager Get()
        {
#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                if (instance == null)
                {
                    instance = new StaticLevelManager();
                    instance.Load();
                }

                return instance;
            }
#endif
            return LoaderManager.instance.levelManager;
        }
    }
}