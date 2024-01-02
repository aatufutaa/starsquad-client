using StarSquad.Lobby.Confirm.Misc;
using StarSquad.Lobby.UI.Screens.Hero;
using StarSquad.Lobby.UI.Screens.HeroPass;
using StarSquad.Lobby.UI.Screens.Level;
using StarSquad.Lobby.UI.Screens.Progression;
using StarSquad.Lobby.UI.Screens.Settings;
using StarSquad.Net.Confirm;
using UnityEngine;

namespace StarSquad.Lobby.UI.Screens
{
    public class ScreenManager : MonoBehaviour
    {
        public NameScreen nameScreen;
        public QueueScreen queue;
        public PartyScreen party;
        public ProfileScreen profile;
        public HeroScreen hero;
        public LevelScreen level;
        public SettingsScreen settings;
        public ProgressionScreen progression;
        public HeroPassScreen heroPass;

        public void Init()
        {
            // init here because they are disabled and may get accessed before enable them
            this.queue.Init();
            this.party.Init();
            this.hero.Init();
            this.settings.Init();
            this.progression.Init();
            this.heroPass.Init();
        }

        public static ScreenManager GetScreenManager()
        {
            return LobbyManager.instance.screenManager;
        }

        public void ShowProfile(string playerId)
        {
            PacketConfirmManager.Get().Send(new RequestProfileOutgoingPacket(playerId));
            this.profile.SetPlayerId(playerId);
            this.profile.Show();
        }
    }
} 