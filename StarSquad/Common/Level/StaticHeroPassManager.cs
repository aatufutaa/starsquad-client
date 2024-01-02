using System;
using System.Collections.Generic;
using System.IO;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using UnityEngine;

namespace StarSquad.Common.Level
{
    public class StaticHeroPassManager
    {
        public List<HeroPassStep> rewards;
        public int heroPassPrice;

        private static string GetHeroPassPath()
        {
#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                return Application.dataPath + "/Editor/TestData/hero_pass.json";
            }
#endif
            return AssetManager.Get().GetFile("hero_pass.json");
        }

        [Serializable]
        public class HeroPassList
        {
            public List<HeroPassStep> rewards;
            public int heroPassPrice;
        }

        [Serializable]
        public class HeroPassStep
        {
            public int tokens;
            public HeroPassReward hero;
            public HeroPassReward free;
        }

        [Serializable]
        public class HeroPassReward
        {
            public int type;
            public int amount;
        }

        public void Load()
        {
            var line = File.ReadAllText(GetHeroPassPath());

            var json = JsonUtility.FromJson<HeroPassList>(line);

            this.rewards = json.rewards;
            this.heroPassPrice = json.heroPassPrice;
        }
    }
}