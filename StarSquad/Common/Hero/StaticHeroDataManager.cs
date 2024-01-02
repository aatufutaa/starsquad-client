using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using StarSquad.Lobby;
using StarSquad.Lobby.UI.Util;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;

namespace StarSquad.Common.Hero
{
    public class StaticHeroDataManager
    {
        public readonly Dictionary<int, StaticHero> heroes = new();

        [Serializable]
        public class StaticHeroList
        {
            public List<StaticHero> heroes;
        }

        [Serializable]
        public class StaticHero
        {
            public int id;
            public string name;
            public string displayName;
            public int rarity;
            public string desc;
            public string bundle;
            public string model;
            public string card;
            public string clip;
            public List<string> particles;
            public List<StaticHeroLevel> levels;
        }

        [Serializable]
        public class StaticHeroLevel
        {
            public int attackDamage;
            public int health;
            public int expPrice;
            public int coinPrice;
        }

        private static string GetHeroesPath()
        {
#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                return Application.dataPath + "/Editor/TestData/heroes.json";
            }
#endif
            return AssetManager.Get().GetFile("heroes.json");
        }

        public void Load()
        {
            var line = File.ReadAllText(GetHeroesPath());

            var json = JsonUtility.FromJson<StaticHeroList>(line);

            foreach (var hero in json.heroes)
            {
                this.heroes.Add(hero.id, hero);
            }
        }

        public StaticHero GetData(int id)
        {
            return this.heroes[id];
        }

        public GameObject LoadHero(int id, Transform parent)
        {
            var staticHero = this.GetData(id);

            Debug.Log("Loading hero from " + staticHero.bundle);
            var bundle = AssetManager.LoadAssetBundle0(staticHero.bundle);
            Debug.Log("bundle loaded " + bundle);
            Debug.Log("loading asset from bundle " + staticHero.model);
            var model = bundle.LoadAsset<GameObject>(staticHero.model);

            var hero = Object.Instantiate(model, parent);

            AssetManager.UnloadAssetBundle0(staticHero.bundle, false);
            return hero;
        }

#if UNITY_EDITOR
        private static StaticHeroDataManager instance;
#endif

        public static StaticHeroDataManager Get()
        {
#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                if (instance == null)
                {
                    instance = new StaticHeroDataManager();
                    instance.Load();
                }

                return instance;
            }
#endif
            return LoaderManager.instance.heroDataManager;
        }

        private IEnumerator LoadAsset(AssetBundle bundle, string name)
        {
            var i = bundle.LoadAssetAsync(name);
            yield return i;
            
            Debug.Log("asset " +name + " -> "+ i.asset);
        }

        public void PlayHeroVideo(int id, CustomVideoPlayer videoPlayer)
        {
            Debug.Log("Play video " + id);

            var staticHero = this.GetData(id);

            var bundle = AssetManager.LoadAssetBundle0(staticHero.bundle);
            var clip = bundle.LoadAsset<VideoClip>(staticHero.clip);

            /*foreach (var name in bundle.assetBundle.GetAllAssetNames())
            {
                LobbyManager.instance.StartCoroutine(this.LoadAsset(bundle.assetBundle, name));
                //var i = bundle.assetBundle.LoadAssetAsync(name);
                //Debug.Log("data " + name);
               // Debug.Log("-> " + i);
            }*/
            
            Debug.Log("Loaded clip from " + staticHero.clip);
            //AssetManager.UnloadAssetBundle0(staticHero.bundle, false);

            Debug.Log("clip " + (clip == null));
            if (clip==null)
            {
                LoaderManager.instance.ShowBadError("Hero failed to load");
                return;
            }

            videoPlayer.PlayVideo(clip);
        }
    }
}