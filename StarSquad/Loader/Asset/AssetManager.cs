using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using StarSquad.Net;
using UnityEngine;
using UnityEngine.Networking;
using Exception = System.Exception;

namespace StarSquad.Loader.Asset
{
    public class AssetManager
    {
        public static readonly string assetCachePath = Application.persistentDataPath + "/AssetCache/";

        private readonly Dictionary<string, MappingData> mappings = new();

        private readonly Dictionary<string, CustomAssetBundle> loadedAssetBundles = new();

        //private readonly Dictionary<string, string> hashCache = new();

        private static void Save(string path, byte[] data)
        {
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

#if UNITY_IOS
            // TODO: do this after save?
            UnityEngine.iOS.Device.SetNoBackupFlag(path);
#endif
            
            File.WriteAllBytes(path, data);
        }

        [Serializable]
        public class AssetInfo
        {
            public string id;
            public string hash;
            public List<string> dep;
        }

        [Serializable]
        public class AssetTable
        {
            public List<AssetInfo> assets;
        }

        public class MappingData
        {
            public AssetInfo info;
            public string path;

            public MappingData(AssetInfo info, string path)
            {
                this.info = info;
                this.path = path;
            }
        }

        public IEnumerator LoadAssets(string url, string assetFile)
        {
            var allowedFiles = new HashSet<string>();
            
            var assetFilePath = assetCachePath + assetFile;
            AssetTable table = null;
            try
            {
                if (File.Exists(assetFilePath))
                {
                    var assetFileData = File.ReadAllText(assetFilePath);
                    table = JsonUtility.FromJson<AssetTable>(assetFileData);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            allowedFiles.Add(assetFilePath);

            if (table == null)
            {
                using (var www = UnityWebRequest.Get(url + "/" + assetFile))
                {
                    yield return www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        throw new Exception("www download error " + www.error);
                    }

                    Save(assetFilePath, www.downloadHandler.data);

                    var assetFileData = www.downloadHandler.text;
                    table = JsonUtility.FromJson<AssetTable>(assetFileData);
                }
            }

            var updates = new List<MappingData>();

            foreach (var info in table.assets)
            {
                var path = assetCachePath + info.hash;
                var data = new MappingData(info, path);

                this.mappings.Add(info.id, data);

                allowedFiles.Add(path);

                if (File.Exists(path))
                {
                    Debug.Log("skipping download of " + info.id + " (" + info.hash + ") it already exsits at " + path);
                    continue;
                }

                updates.Add(data);
            }

            // delete not used files
            foreach (var file in Directory.GetFiles(assetCachePath))
            {
                if (allowedFiles.Contains(file)) continue;
                Debug.Log("Deleting not used asset file " + file);
                File.Delete(file);
            }
            
            var updateCount = updates.Count;

            if (updateCount > 0)
            {
                LoaderManager.instance.SetDownloading();
                NetworkManager.GetNet().connectionManager.SafeDisconnect("downloading content");

                yield return null;
                
                var progress = 0;

                foreach (var update in updates)
                {
                    using (var www = UnityWebRequest.Get(url + "/" + update.info.hash))
                    {
                        var op = www.SendWebRequest();

                        while (!op.isDone)
                        {
                            var subProgress = op.progress;
                            LoaderManager.instance.downloadingProgress = (progress + subProgress) / updateCount;
                            yield return null;
                        }

                        if (www.result != UnityWebRequest.Result.Success)
                        {
                            throw new Exception("www download error " + www.error);
                        }

                        var data = www.downloadHandler.data;

                        Save(update.path, data);
                        if (update.info.id.Equals("custom_loader_bundle"))
                        {
                            PlayerPrefs.SetString("loader", update.path);
                            PlayerPrefs.Save();
                        }

                        Debug.Log("downloading work");
                    }

                    ++progress;
                    LoaderManager.instance.downloadingProgress = progress / (float)updateCount;
                }

                yield return new WaitForSeconds(1f);
            }

            if (updateCount > 0)
                LoaderManager.instance.Reload();
            else
                LoaderManager.instance.OnAssetsLoaded();
        }

        public string GetFile(string name)
        {
            return this.mappings.TryGetValue(HashAssetName(name), out var val) ? val.path : null;
        }

        public CustomAssetBundle LoadAssetBundle(string name)
        {
            return this.LoadAssetBundleWithHashedName(this.HashAssetName(name));
        }

        private CustomAssetBundle LoadAssetBundleWithHashedName(string name)
        {
            if (this.loadedAssetBundles.TryGetValue(name, out var loaded))
                return loaded;

            if (!this.mappings.TryGetValue(name, out var mapping)) return null;

            // load bundle
            var bundle = AssetBundle.LoadFromFile(mapping.path);
            var custom = new CustomAssetBundle(bundle);
            this.loadedAssetBundles.Add(name, custom);

            // load dependencies after loading bundle so we dont get stuck in loop
            var dependencies = mapping.info.dep;
            foreach (var dependency in dependencies)
            {
                this.LoadAssetBundle(dependency);
            }

            return custom;
        }

        public void UnloadAssetBundle(string name, bool unloadLoadedObjects)
        {
            if (!this.loadedAssetBundles.TryGetValue(this.HashAssetName(name), out var loaded))
                return;

            loaded.assetBundle.Unload(unloadLoadedObjects);

            this.loadedAssetBundles.Remove(this.HashAssetName(name));
        }

        private string HashAssetName(string name)
        {
            /*if (this.hashCache.TryGetValue(name, out var val)) return val;
            using (var md5 = MD5.Create())
            {
                var input = Encoding.UTF8.GetBytes(name);
                var bytes = md5.ComputeHash(input);

                var hex = new StringBuilder(bytes.Length * 2);
                foreach (byte b in bytes)
                    hex.AppendFormat("{0:x2}", b);
                var hash = hex.ToString();
                this.hashCache.Add(name, hash);
                return hash;
            }*/
            return name;
        }

        public void UnloadAllAssetBundles()
        {
            /*foreach (var bundle in this.loadedAssetBundles.Values)
            {
                bundle.Unload(true);
            }*/

            AssetBundle.UnloadAllAssetBundles(true);
        }

        public static AssetManager Get()
        {
            return LoaderManager.instance.assetManager;
        }

        public static CustomAssetBundle LoadAssetBundle0(string name)
        {
#if UNITY_EDITOR
            return new CustomAssetBundle(null);
#endif
            return Get().LoadAssetBundle(name);
        }

        public static void UnloadAssetBundle0(string name, bool unloadLoadedObjects)
        {
#if UNITY_EDITOR
            return;
#else
            Get().UnloadAssetBundle(name, unloadLoadedObjects);
#endif
        }
    }
}