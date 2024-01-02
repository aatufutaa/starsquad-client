using System;
using System.Collections;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using StarSquad.Net.Session;
using UnityEngine;

namespace StarSquad.Net.Packet.Login
{
    public class HelloIncomingPacket : IncomingPacket
    {
        private const int Ok = 0;
        private const int Maintenance = 1;
        private const int Update = 2;
        private const int Fail = 3;

        private int response;
        private string msg;
        private bool hasSignature;
        private string signature;

        public void Read(ByteBuf buf)
        {
            this.response = buf.ReadByte();
            this.msg = buf.ReadString();
            this.hasSignature = buf.ReadBool();
            if (this.hasSignature)
            {
                this.signature = buf.ReadString();
            }
        }

        [Serializable]
        public class AssetResponse
        {
            public string url;
            public string assetFile;
        }

        [Serializable]
        public class MaintenanceResponse
        {
            public string title;
            public string msg;
        }

        [Serializable]
        public class UpdateResponse
        {
            public string url;
        }

        [Serializable]
        public class FailResponse
        {
            public string msg;
        }

        private IEnumerator StartLoading()
        {
            // download and check assets from content server
            var assetManager = new AssetManager();
            LoaderManager.instance.assetManager = assetManager;

            var json = JsonUtility.FromJson<AssetResponse>(this.msg);

            var url = json.url;
            if (url.Contains("127.0.0.1") && Application.platform == RuntimePlatform.Android)
            {
                url = url.Replace("127.0.0.1", "10.0.2.2");
            }

            yield return assetManager.LoadAssets(url, json.assetFile);
        }

        public void Handle()
        {
            NetworkManager.GetNet().sessionManager.AssertState(LoginState.Hello);

            Debug.Log("Read hello " + this.response + " " + this.msg);

            if (this.hasSignature)
            {
                // TODO: verify signing
            }

            switch (this.response)
            {
                case Ok:
                {
                    if (!this.hasSignature)
                    {
                        Debug.LogWarning("ok but did no receive signature");
                        return;
                    }
                    
                    LoaderManager.instance.StartCoroutine(this.StartLoading());
                    return;
                }

                case Maintenance:
                {
                    Debug.Log("Maintenance: " + this.msg);

                    var response = JsonUtility.FromJson<MaintenanceResponse>(this.msg);

                    NetworkManager.GetNet().connectionManager.SafeDisconnect("maintenance");

                    LoaderManager.instance.nativeDialogManager.ShowDialog(response.title, response.msg, "Okay",
                        button => { LoaderManager.instance.Reload(); });
                    return;
                }

                case Update:
                {
                    Debug.Log("Update available. " + this.msg);

                    if (!this.hasSignature)
                    {
                        Debug.LogWarning("update available but did not receive signature");
                        return;
                    }

                    var response = JsonUtility.FromJson<UpdateResponse>(this.msg);

                    NetworkManager.GetNet().connectionManager.SafeDisconnect("update");

                    LoaderManager.instance.nativeDialogManager.ShowDialog("Update Available",
                        "Good news! New update is available.", "Update",
                        button => { Application.OpenURL(response.url); });

                    return;
                }

                case Fail:
                {
                    Debug.Log("Login error" + this.msg);

                    var response = JsonUtility.FromJson<FailResponse>(this.msg);

                    NetworkManager.GetNet().connectionManager.SafeDisconnect("hello login failed");

                    LoaderManager.instance.nativeDialogManager.ShowDialog("Login Error", response.msg, "Try again",
                        button => { LoaderManager.instance.Reload(); });
                    return;
                }
            }
        }
    }
}