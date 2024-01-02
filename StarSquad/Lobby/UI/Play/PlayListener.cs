using StarSquad.Common.Hero;
using StarSquad.Loader;
using StarSquad.Lobby.Confirm.Packet;
using StarSquad.Lobby.Hero;
using StarSquad.Lobby.UI.Heroes;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Lobby.UI.Util;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Play
{
    public class PlayListener : MonoBehaviour
    {
        //public PlayerHero playerHero;
        //public PlayerHero player2Hero;
        //public PlayerHero player3Hero;

        //public Transform heroParent;

        //private GameObject hero;

        //public CustomVideoPlayer customVideoPlayer;

        public TMP_Text trophiesText;
        public TMP_Text levelText;
        
        public void UpdateHero(int hero)
        {
           // if (this.hero)
           // {
           //     Destroy(this.hero);
           // }
            //this.hero = StaticHeroDataManager.Get().LoadHero(hero, this.heroParent);
            //StaticHeroDataManager.Get().PlayHeroVideo(hero, this.customVideoPlayer);
            HeroPlayer.Get().Play(hero, true);

            var data = HeroManager.Get().GetItem(hero);
            this.trophiesText.text = "" + data.GetTrophies();
            this.levelText.text = "" + data.GetLevel();
        } 
        
        public void HandlePlay()
        {
            Debug.Log("Play button pressed");

            if (LoaderManager.IsUsingNet())
            {
                PacketConfirmManager.Get().Send(new JoinQueueOutgoingPacket(JoinQueueOutgoingPacket.QueueType.TowerWars));
            }

            ScreenManager.GetScreenManager().queue.Show();
        }

        public void HandleParty()
        {
            Debug.Log("Party button pressed");
            
            ScreenManager.GetScreenManager().party.Show();
        }

        public static PlayListener Get()
        {
            return LobbyManager.instance.playListener;
        }
    }
}