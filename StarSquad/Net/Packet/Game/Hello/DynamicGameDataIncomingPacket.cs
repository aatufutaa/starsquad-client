using System.Collections.Generic;
using StarSquad.Game;
using StarSquad.Game.Mode;
using StarSquad.Loader;
using UnityEngine;

namespace StarSquad.Net.Packet.Game.Hello
{
    public class DynamicGameDataIncomingPacket : IncomingPacket
    {
        private int currentTick;

        private GameState gameState;
        
        private DynamicGameData dynamicGameData;
        
        private List<PlayerInfo> players;
        
        public void Read(ByteBuf buf)
        {
            this.currentTick = buf.ReadShort();

            this.gameState = (GameState)buf.ReadByte();
            
            var world = GameManager.instance.gameWorld;
            var game = world.game;
            this.dynamicGameData = game.GetDynamicGameData();
            this.dynamicGameData.Read(buf);
            
            var count = buf.ReadByte();
            this.players = new List<PlayerInfo>(count);
            for (var i = 0; i < count; i++)
            {
                var playerInfo = new PlayerInfo();
                playerInfo.Read(buf);
                this.players.Add(playerInfo);
            }
            
            // map data
            /*var blockManager = Move.instance.gameWorld.blockManager;
            var dirtyBlocks = buf.ReadShort();
            for (var i = 0; i < dirtyBlocks; i++)
            {
                var x = buf.ReadByte();
                var y = buf.ReadByte();
                var block = blockManager.GetSpecialBlock(x, y);
                if (block == null) throw new Exception("cant find block when adding dynamic data at " + x + ", " + y);
                block.ReadDynamic(buf);
            }
            */
        }

        public void Handle()
        {
            Debug.Log("Read dynamic game data");

            Debug.Log("Tick " + this.currentTick);

            var world = GameManager.instance.gameWorld;

            world.game.UpdateState(this.gameState);
            
            this.dynamicGameData.Handle(world);

            foreach (var playerInfo in this.players)
            {
                var player = world.GetPlayer(playerInfo.entityId);
                player.SetDynamicData(playerInfo.health, playerInfo.x, playerInfo.y);
            }

            world.dynamicDataLoaded = true;
            
            if (!NetworkManager.GetNet().connectionManager.IsFallbackToTcp())
            {
                // tell server client is ready to accept udp packets
                NetworkManager.GetNet().connectionManager.SendPacket(
                    new UdpReadyOutgoingPacket(UdpReadyOutgoingPacket.ReadyToAcceptPackets));
            }

            LoaderManager.instance.HideLoader();
        }

        public class PlayerInfo
        {
            public int entityId;
            public int health;

            public float x;
            public float y;

            public void Read(ByteBuf buf)
            {
                this.entityId = buf.ReadByte();
                this.health = buf.ReadShort();

                this.x = buf.ReadFloat();
                this.y = buf.ReadFloat();
            }
        }
    }
}