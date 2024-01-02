using System.Collections;
using System.Collections.Generic;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using StarSquad.Lobby;
using StarSquad.Lobby.Confirm.Friend;
using StarSquad.Lobby.Party;
using StarSquad.Lobby.UI;
using StarSquad.Lobby.UI.Header;
using StarSquad.Lobby.UI.Heroes;
using StarSquad.Lobby.UI.Play;
using StarSquad.Lobby.UI.Quests;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Packet.Play;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StarSquad.Net.Packet.Lobby.Hello
{
    public class LobbyDataIncomingPacket : IncomingPacket
    {
        private bool askForName;
        private string name;

        private int level;
        private int levelProgress;
        private int maxLevelProgress;
        private int claimedLevelRewardIndex;

        private int seasonEndTime;
        private int heroTokens;
        private bool hasHeroPass;
        private int heroPassHeroClaimIndex;
        private int heroPassFreeClaimIndex;

        private int gems;
        private int coins;

        private int expCommon;
        private int expRare;
        private int expLegendary;

        private Dictionary<int, HeroInfo> heroes;
        private int selectedHero;

        private int totalTrophies;
        private int highestTrophies;

        private int giveTrophies;
        private int giveTokens;

        private List<AddFriendIncomingPacket.Friend> friends;
        private List<AddFriendIncomingPacket.Friend> outgoingInvites;
        private List<AddFriendIncomingPacket.Friend> incomingInvites;

        private bool allowFriendRequests;

        private List<QuestInfo> quests;

        private HashSet<int> claimedProgression;

        private bool inQueue;

        private bool inParty;
        private string partyCode;
        private string partyLeaderId;
        private List<PartyMember> members;

        private int location;

        public class HeroInfo
        {
            public int id;
            public int level;
            public int rating;

            public void Read(ByteBuf buf)
            {
                this.id = buf.ReadByte();
                this.level = buf.ReadByte();
                this.rating = buf.ReadShort();
            }
        }

        public class QuestInfo
        {
            public int id;
            public int amount;
            public int claimIndex;
        }

        public void Read(ByteBuf buf)
        {
            this.askForName = buf.ReadBool();
            if (!this.askForName)
                this.name = buf.ReadString();

            this.level = buf.ReadByte();
            this.levelProgress = buf.ReadShort();
            this.maxLevelProgress = buf.ReadShort();
            this.claimedLevelRewardIndex = buf.ReadByte();

            this.seasonEndTime = buf.ReadInt();
            this.heroTokens = buf.ReadShort();
            this.hasHeroPass = buf.ReadBool();
            if (this.hasHeroPass)
            {
                this.heroPassHeroClaimIndex = buf.ReadByte();
            }

            this.heroPassFreeClaimIndex = buf.ReadByte();

            this.gems = buf.ReadInt();
            this.coins = buf.ReadShort();

            this.expCommon = buf.ReadShort();
            this.expRare = buf.ReadShort();
            this.expLegendary = buf.ReadShort();

            var heroCount = buf.ReadByte();
            this.heroes = new Dictionary<int, HeroInfo>(heroCount);
            for (var i = 0; i < heroCount; i++)
            {
                var hero = new HeroInfo();
                hero.Read(buf);
                this.heroes.Add(hero.id, hero);
            }

            this.selectedHero = buf.ReadByte();

            this.totalTrophies = buf.ReadShort();
            this.highestTrophies = buf.ReadShort();

            this.giveTrophies = buf.ReadShort();
            this.giveTokens = buf.ReadShort();

            var friendsCount = buf.ReadByte();
            this.friends = new List<AddFriendIncomingPacket.Friend>(friendsCount);
            for (var i = 0; i < friendsCount; i++)
            {
                var friend = new AddFriendIncomingPacket.Friend();
                friend.Read(buf);
                this.friends.Add(friend);
            }

            friendsCount = buf.ReadByte();
            this.outgoingInvites = new List<AddFriendIncomingPacket.Friend>(friendsCount);
            for (var i = 0; i < friendsCount; i++)
            {
                var friend = new AddFriendIncomingPacket.Friend();
                friend.Read(buf);
                this.outgoingInvites.Add(friend);
            }

            friendsCount = buf.ReadByte();
            this.incomingInvites = new List<AddFriendIncomingPacket.Friend>(friendsCount);
            for (var i = 0; i < friendsCount; i++)
            {
                var friend = new AddFriendIncomingPacket.Friend();
                friend.Read(buf);
                this.incomingInvites.Add(friend);
            }

            this.allowFriendRequests = buf.ReadBool();

            var questCount = buf.ReadByte();
            this.quests = new List<QuestInfo>(questCount);
            for (var i = 0; i < questCount; i++)
            {
                var quest = new QuestInfo();
                quest.id = buf.ReadByte();
                quest.amount = buf.ReadShort();
                quest.claimIndex = buf.ReadByte();
                this.quests.Add(quest);
            }

            var progressionCount = buf.ReadByte();
            this.claimedProgression = new HashSet<int>(progressionCount);
            for (var i = 0; i < progressionCount; i++)
            {
                this.claimedProgression.Add(buf.ReadByte());
            }

            this.inQueue = buf.ReadBool();

            this.inParty = buf.ReadBool();
            if (this.inParty)
            {
                this.partyCode = buf.ReadString();

                this.partyLeaderId = buf.ReadString();

                var count = buf.ReadByte();
                this.members = new List<PartyMember>(count);
                for (var i = 0; i < count; i++)
                {
                    var member = new PartyMember();
                    member.Read(buf);
                    this.members.Add(member);
                }
            }

            this.location = buf.ReadByte();
        }

        private IEnumerator LoadScene()
        {
            //yield return null;
            yield return new WaitForSeconds(0.5f);

            Debug.Log("Loading scene");

            var bundle = AssetManager.LoadAssetBundle0("lobby_scene");
            var op = SceneManager.LoadSceneAsync(bundle.GetScenePath("Assets/Remote/Lobby/Lobby.unity"));
            while (!op.isDone)
            {
                LoaderManager.instance.subProgress = op.progress * 3;
                yield return null;
            }

            yield return null;

            Debug.Log("Scene loaded");

            yield return null;

            LoaderManager.instance.subProgress = 0f;
            LoaderManager.instance.UpdateStage(8);

            yield return null; // set data next frame

            var lobbyData = new LobbyData();
            LobbyManager.instance.lobbyData = lobbyData;

            // what is your name
            if (this.askForName)
            {
                ScreenManager.GetScreenManager().nameScreen.Show();
            }
            else
            {
                Object.Destroy(ScreenManager.GetScreenManager().nameScreen.gameObject);
            }

            // header
            // load level before setting values to header
            var level = ScreenManager.GetScreenManager().level;
            level.SetClaimData(this.claimedLevelRewardIndex);

            var header = HeaderManager.Get();
            header.SetLevel(this.level, this.levelProgress, this.maxLevelProgress);
            header.SetGems(this.gems);
            header.SetCoins(this.coins);

            header.SetExpCommon(this.expCommon);
            header.SetExpRare(this.expRare);
            header.SetExpLegendary(this.expLegendary);

            // heropass
            ScreenManager.GetScreenManager().heroPass.SetData(this.seasonEndTime, this.heroTokens,
                this.heroPassHeroClaimIndex, this.heroPassFreeClaimIndex, this.hasHeroPass);

            // heroes
            var heroManager = HeroManager.Get();
            heroManager.AddHeroes(this.heroes);

            PlayListener.Get().UpdateHero(this.selectedHero);

            // trophies
            lobbyData.totalTrophies = this.totalTrophies;

            // friends
            var friendsListener = LobbyManager.instance.friendsListener;
            foreach (var friend in this.friends)
            {
                friendsListener.AddFriend(friend.playerId, friend.name, friend.rating, 0);
            }

            foreach (var friend in this.outgoingInvites)
            {
                friendsListener.AddOutgoingInvite(friend, false);
            }

            friendsListener.UpdateOutgoingText();

            foreach (var friend in this.incomingInvites)
            {
                friendsListener.AddIncomingInvite(friend, false);
            }

            friendsListener.UpdateIncomingText();

            // queue
            if (this.inQueue)
            {
                ScreenManager.GetScreenManager().queue.Show();
            }

            // party
            if (this.inParty)
            {
                // player is in party -> create party silently
                var playerIsLeader = NetworkManager.GetNet().sessionManager.playerId == this.partyLeaderId;
                LobbyManager.instance.partyManager.CreateParty(this.partyCode, true, playerIsLeader);
                // join players silent
                foreach (var member in this.members)
                {
                    var memberIsLeader = member.playerId == this.partyLeaderId;
                    LobbyManager.instance.partyManager.OtherPlayerJoinParty(member, true, memberIsLeader);
                }
            }

            var settings = ScreenManager.GetScreenManager().settings;
            if (!this.allowFriendRequests)
                settings.friendRequestsButton.SetEnabled(false);
            settings.locationScreen.SetSelectedLocation(this.location);

            foreach (var quest in this.quests)
            {
                QuestsManager.Get().UpdateItem(quest);
            }

            ScreenManager.GetScreenManager().progression
                .SetData(this.totalTrophies, this.highestTrophies, this.claimedProgression);
            ScreenManager.GetScreenManager().progression.progression
                .SetTrophies(this.totalTrophies - this.giveTrophies);
            if (this.giveTrophies > 0)
                ScreenManager.GetScreenManager().progression.progression.afterGameReward
                    .GiveTrophies(this.giveTrophies);
            ScreenManager.GetScreenManager().heroPass.heroPassButton.SetData(this.heroTokens);
            if (this.giveTokens > 0)
                ScreenManager.GetScreenManager().heroPass.heroPassButton.AddTokens(this.giveTokens);

            if (!this.askForName)
                lobbyData.OnNameChanged(this.name);

            LoaderManager.instance.UpdateStage(9);

            // wait for hero to be loaded
            while (!LobbyManager.instance.heroPlayer.customVideoPlayer.IsLoaded())
            {
                yield return null;
            }
            
            NetworkManager.GetNet().sessionManager.dataLoaded = true; // static data load done

            NetworkManager.GetNet().connectionManager.SendPacket(new DataLoadedOutgoingPacket());
        }

        public void Handle()
        {
            LoaderManager.instance.UpdateStage(5);
            LoaderManager.instance.StartCoroutine(this.LoadScene()); // load scene next frame so loader updates
        }
    }
}