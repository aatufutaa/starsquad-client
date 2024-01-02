using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StarSquad.Loader.Asset
{
    public class CustomAssetBundle
    {
        public AssetBundle assetBundle;

        public CustomAssetBundle(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
        }

        public string GetScenePath(string def)
        {
#if UNITY_EDITOR
            return def;
# endif
            return this.assetBundle.GetAllScenePaths()[0];
        }

        public T LoadAsset<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<T>(path);
#endif
            return this.assetBundle.LoadAsset<T>(path);
        }
        
        public T[] LoadAllAssets<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            var data = new List<T>();
            foreach (var a in assets)
            {
                if (a is T t)
                {
                    data.Add(t);
                }
            }
            return data.ToArray();
#endif
            return this.assetBundle.LoadAssetWithSubAssets<T>(path);
        }
    }
}