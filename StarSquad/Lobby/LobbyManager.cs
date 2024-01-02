using System.Collections.Generic;
using StarSquad.Common.Audio;
using StarSquad.Common.Hero;
using StarSquad.Loader;
using StarSquad.Loader.Intro;
using StarSquad.Lobby.Hero;
using StarSquad.Lobby.Party;
using StarSquad.Lobby.UI;
using StarSquad.Lobby.UI.Friends;
using StarSquad.Lobby.UI.Header;
using StarSquad.Lobby.UI.Heroes;
using StarSquad.Lobby.UI.Nav;
using StarSquad.Lobby.UI.Panel;
using StarSquad.Lobby.UI.Play;
using StarSquad.Lobby.UI.Quests;
using StarSquad.Lobby.UI.Reward;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Lobby.UI.Shop;
using StarSquad.Lobby.UI.Tooltip;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace StarSquad.Lobby
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager instance;

        public ScreenManager screenManager;
        public PlayListener playListener;
        public FriendsListener friendsListener;
        public HeaderManager headerManager;

        public Material grayscaleMaterial;

        public PartyManager partyManager;

        public LobbyData lobbyData;

        public TooltipManager tooltipManager;

        public HeroManager heroManager;

        public RewardManager rewardManager;

        public PanelManager panelManager;

        public NavManager navManager;
        public ShopManager shopManager;

        public AudioManager audioManager;

        public QuestsManager questsManager;

        public HeroPlayer heroPlayer;

        private void Awake()
        {
            instance = this;

            this.partyManager = new PartyManager();

#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                Application.targetFrameRate = 60;
                this.gameObject.AddComponent<EventSystem>();
                this.gameObject.AddComponent<InputSystemUIInputModule>();

                this.lobbyData = new LobbyData();

                PreLoader.Init();
                PreLoader.LoadSettings();
                this.audioManager.Refresh();
            }
#endif

            this.tooltipManager = new TooltipManager();
            
            this.screenManager.Init();
            this.questsManager.Init();
        }

        private void Start()
        {
            this.audioManager.PlayMusic("lobby", true);

#if UNITY_EDITOR
            if (LoaderManager.IsUsingNet()) return;

            this.playListener.UpdateHero(0);
#endif
        }

        private void Stop0()
        {
            Debug.Log("stop lobby manager");
        }

        public static void Stop()
        {
            if (instance == null) return;

            instance.Stop0();
            instance = null;
        }

        public static bool IsLobby()
        {
            return !ReferenceEquals(instance, null);
        }

        public void PauseLobby()
        {
            this.heroPlayer.customVideoPlayer.PausePlayer();
            
            this.audioManager.MuteAll();
        }

        public void ResumeLobby()
        {
            this.heroPlayer.customVideoPlayer.ResumePlayer();
            
            this.audioManager.UnmuteAll();
            
            
            
        }
    }
}