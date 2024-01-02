using System;
using System.IO;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using UnityEngine;

namespace StarSquad.Common.Level
{
    public class StaticShopManager
    {
        public ShopData shopData;

        private static string GetShopPath()
        {
#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                return Application.dataPath + "/Editor/TestData/shop.json";
            }
#endif
            return AssetManager.Get().GetFile("shop.json");
        }

        [Serializable]
        public class ShopList
        {
            public ShopData shop;
        }

        [Serializable]
        public class ShopData
        {
            public float commonManaAsGems;
            public float epicManaAsGems;
            public float legendaryManaAsGems;
            public float coinAsGems;
            public float heroTokenAsGems;
        }

        public void Load()
        {
            var line = File.ReadAllText(GetShopPath());

            var json = JsonUtility.FromJson<ShopList>(line);

            this.shopData = json.shop;

            Debug.Log("commonManaAsGems " + this.shopData.commonManaAsGems);
            Debug.Log("epicManaAsGems " + this.shopData.epicManaAsGems);
            Debug.Log("legendaryManaAsGems " + this.shopData.legendaryManaAsGems);
            Debug.Log("coinAsGems " + this.shopData.coinAsGems);
            Debug.Log("heroTokenAsGems " + this.shopData.heroTokenAsGems);
        }

#if UNITY_EDITOR
        private static StaticShopManager instance;
#endif

        public static StaticShopManager Get()
        {
#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                if (instance != null) return instance;
                instance = new StaticShopManager();
                instance.Load();
                return instance;
            }
#endif
            return LoaderManager.instance.shopManager;
        }
    }
}