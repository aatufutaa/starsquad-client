using System.Collections;
using System.Collections.Generic;
using StarSquad.Game;
using StarSquad.Game.Block;
using StarSquad.Game.Block.Blocks;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using StarSquad.Net.Packet.Play;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StarSquad.Net.Packet.Game.Hello
{
    public class GameDataIncomingPacket : IncomingPacket
    {
        private int udpPort;

        private GameType gameType;

        private int thePlayer;
        private List<PlayerInfo> players;

        private string theme;
        private int mapSizeX;
        private int mapSizeY;
        private List<BlockBase> blocks;

        public void Read(ByteBuf buf)
        {
            this.udpPort = buf.ReadUShort();

            this.gameType = (GameType)buf.ReadByte();

            this.thePlayer = buf.ReadByte();
            var count = buf.ReadByte();
            this.players = new List<PlayerInfo>(count);
            for (var i = 0; i < count; i++)
            {
                var playerInfo = new PlayerInfo();
                playerInfo.Read(buf);
                this.players.Add(playerInfo);
            }

            // map
            this.theme = buf.ReadString();
            this.mapSizeX = buf.ReadByte();
            this.mapSizeY = buf.ReadByte();
            var blockCount = buf.ReadShort();
            this.blocks = new List<BlockBase>(blockCount);
            for (var i = 0; i < blockCount; i++)
            {
                var id = buf.ReadByte();
                var block = BlockRegistry.ReadBlock(id, buf);
                this.blocks.Add(block);
            }
        }

        private IEnumerator LoadData()
        {
            // load scene
            var bundle = AssetManager.LoadAssetBundle0("game_scene");
            var op = SceneManager.LoadSceneAsync(bundle.GetScenePath("Assets/Remote/Game/Game.unity"));
            while (!op.isDone)
            {
                LoaderManager.instance.subProgress = op.progress * 3;
                yield return null;
            }

            yield return null;

            LoaderManager.instance.subProgress = 0f;
            LoaderManager.instance.UpdateStage(8);

            Debug.Log("Scene loaded!");

            NetworkManager.GetNet().connectionManager.InitUdp(this.udpPort);

            yield return null;

            var world = GameManager.instance.gameWorld;

            world.Init(this.gameType, this.theme, this.mapSizeX, this.mapSizeY);
            foreach (var block in this.blocks)
            {
                world.blockManager.AddBlock(block);
            }

            world.blockManager.OnBlocksFinished();

            foreach (var playerInfo in this.players)
            {
                world.AddPlayer(playerInfo, playerInfo.entityId == this.thePlayer);
            }

            world.staticDataLoaded = true;

            NetworkManager.GetNet().sessionManager.dataLoaded = true; // static data loaded

            NetworkManager.GetNet().connectionManager.SendPacket(new DataLoadedOutgoingPacket());
        }

        public void Handle()
        {
            Debug.Log("Game data received. Loading scene...");

            LoaderManager.instance.UpdateStage(5);

            // load data on next tick to wait scene to be gameobject
            LoaderManager.instance.StartCoroutine(this.LoadData());
        }

        public class PlayerInfo
        {
            public int entityId;
            public string name;
            public int hero;
            public int skin;
            public bool team;
            public int maxHealth;

            public void Read(ByteBuf buf)
            {
                this.entityId = buf.ReadByte();
                this.name = buf.ReadString();
                this.hero = buf.ReadByte();
                this.skin = buf.ReadByte();
                this.team = buf.ReadBool();
                this.maxHealth = buf.ReadShort();
            }
        }
    }
}