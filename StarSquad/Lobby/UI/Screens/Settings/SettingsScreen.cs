using System;
using System.Collections.Generic;
using System.IO;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using StarSquad.Loader.Intro;
using StarSquad.Lobby.Confirm.Misc;
using StarSquad.Lobby.UI.Button;
using StarSquad.Net;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Screens.Settings
{
    public class SettingsScreen : GameScreen
    {
        public TMP_Text playerIdText;
        public TMP_Text versionText;

        public SettingsButton musicButton;
        public SettingsButton soundEffectsButton;
        public SettingsButton friendRequestsButton;

        public TMP_Text locationButtonText;
        public LocationSettingsScreen locationScreen;

        public CustomButton helpAndSupport;
        public CustomButton termsOfServiceButton;
        public CustomButton privacyButton;
        
        private static string GetSettingsPath()
        {
#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                return Application.dataPath + "/Editor/TestData/settings.json";
            }
#endif
            return AssetManager.Get().GetFile("settings.json");
        }

        [Serializable]
        public class SettingsJsonData
        {
            public List<string> locations;
            public string helpAndSupport;
            public string termsOfService;
            public string privacy;
        }

        private void ReadSettings()
        {
            var line = File.ReadAllText(GetSettingsPath());
            var json = JsonUtility.FromJson<SettingsJsonData>(line);
            
            this.locationScreen.Init(json.locations);
            
            this.helpAndSupport.onClick.AddListener(() => Application.OpenURL(json.helpAndSupport));
            this.termsOfServiceButton.onClick.AddListener(() => Application.OpenURL(json.termsOfService));
            this.privacyButton.onClick.AddListener(() => Application.OpenURL(json.privacy));
        }
        
        public void Init()
        {
            if (LoaderManager.IsUsingNet())
                this.playerIdText.text =
                    string.Format(this.playerIdText.text, NetworkManager.GetNet().sessionManager.playerId);

            this.versionText.text = string.Format(this.versionText.text, LoaderConstants.VersionMajor,
                LoaderConstants.VersionMinor);

            this.musicButton.onClick.AddListener(this.HandleMusic);
            this.soundEffectsButton.onClick.AddListener(this.HandleSoundEffects);
            this.friendRequestsButton.onClick.AddListener(this.HandleFriendRequests);

            this.ReadSettings();
            
#if !UNITY_EDITOR
            this.InitSettings();
#endif
        }

#if UNITY_EDITOR
        private void Start()
        {
            this.InitSettings();
        }
#endif

        private void InitSettings()
        {
            if (!PreLoader.music)
                this.musicButton.SetEnabled(false);
            if (!PreLoader.sounds)
                this.soundEffectsButton.SetEnabled(false);
        }

        private void HandleMusic()
        {
            LobbyManager.instance.audioManager.OnMusicEnabled(this.musicButton.state);
        }

        private void HandleSoundEffects()
        {
            LobbyManager.instance.audioManager.OnSoundsEnabled(this.soundEffectsButton.state);
        }

        private void HandleFriendRequests()
        {
            this.SendSettings();
        }
        
        public void SendSettings() {
            PacketConfirmManager.Get().Send(new UpdateSettingsOutgoingPacket(this.friendRequestsButton.state,
                this.locationScreen.location));
        }
    }
}