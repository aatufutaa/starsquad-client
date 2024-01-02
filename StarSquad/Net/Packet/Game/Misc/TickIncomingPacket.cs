using System;
using System.Collections.Generic;
using StarSquad.Game;
using StarSquad.Game.Hero;
using StarSquad.Game.Mode;
using UnityEngine;

namespace StarSquad.Net.Packet.Game.Misc
{
    public class TickIncomingPacket : IncomingPacket
    {
        private static Dictionary<int, Type> tickPackets = new Dictionary<int, Type>();

        static TickIncomingPacket()
        {
            tickPackets.Add(0, typeof(AddProjectilePacket));
            tickPackets.Add(1, typeof(RemoveProjectilePacket));
            tickPackets.Add(2, typeof(BlockHitPacket));
            tickPackets.Add(3, typeof(DamageEntityPacket));
            tickPackets.Add(4, typeof(RespawnPlayerPacket));
            tickPackets.Add(5, typeof(TutorialPacket));
            tickPackets.Add(6, typeof(GameStatePacket));
            tickPackets.Add(7, typeof(GameStartingPacket));
            tickPackets.Add(8, typeof(GameWinnersPacket));
            tickPackets.Add(9, typeof(YouDiedPacket));
        }

        private int serverTick;
        private List<TickPacket> packets;
        private List<EntityData> fallback;

        public class EntityData
        {
            public int entityId;
            public float x;
            public float y;
            public float rot;

            public void Read(ByteBuf buf)
            {
                this.entityId = buf.ReadByte();
                this.x = buf.ReadFloat();
                this.y = buf.ReadFloat();
                this.rot = buf.ReadFloat();
            }
        }
            
        public void Read(ByteBuf buf)
        {
            this.serverTick = buf.ReadShort();

            var count = buf.ReadByte();
            this.packets = new List<TickPacket>(count);
            for (var i = 0; i < count; i++)
            {
                var id = buf.ReadByte();

                if (!tickPackets.TryGetValue(id, out var type))
                    throw new Exception("cant find tick packet " + id);

                var packet = (TickPacket)Activator.CreateInstance(type);
                packet.Read(buf);
                
                this.packets.Add(packet);
            }
            
            var fallbackCount = buf.ReadByte();
            this.fallback = new List<EntityData>(fallbackCount);
            for (var i = 0; i < fallbackCount; i++)
            {
                var entityData = new EntityData();
                entityData.Read(buf);
                this.fallback.Add(entityData);
            }
        }

        public void Handle()
        {
            foreach (var packet in this.packets)
            {
                packet.Handle();
            }

            var world = GameManager.instance.gameWorld;
            world.HandleEntityData(this.fallback);
            world.game.UpdateServerTick(this.serverTick);
        }

        public interface TickPacket
        {
            void Read(ByteBuf buf);
            void Handle();
        }

        public class AddProjectilePacket : TickPacket
        {
            private int entityId;
            private int projectileId;

            private float x;
            private float y;

            private float velX;
            private float velY;

            public void Read(ByteBuf buf)
            {
                this.entityId = buf.ReadByte();
                this.projectileId = buf.ReadByte();

                this.x = buf.ReadFloat();
                this.y = buf.ReadFloat();

                this.velX = buf.ReadFloat();
                this.velY = buf.ReadFloat();
            }

            public void Handle()
            {
                Debug.Log("add projectile at " + this.x + " " + this.y + " " + this.velX + " " + this.velY + " " + this.entityId);

                var world = GameManager.instance.gameWorld;
                var player = world.thePlayer;

                ((HeroProjectile)player.hero).OnPostAttack(this.projectileId, this.x, this.y, new Vector2(this.velX, this.velY));
            }
        }

        public class RemoveProjectilePacket : TickPacket
        {
            private int entityId;
            private int projectileId;

            private float x;
            private float y;

            public void Read(ByteBuf buf)
            {
                this.entityId = buf.ReadByte();
                this.projectileId = buf.ReadByte();

                this.x = buf.ReadFloat();
                this.y = buf.ReadFloat();
            }

            public void Handle()
            {
                Debug.Log("remove projectile " + this.entityId + " " + this.projectileId);
                
                var world = GameManager.instance.gameWorld;
                var player = world.thePlayer;
                
                ((HeroProjectile)player.hero).Remove(this.projectileId, this.x, this.y);
            }
        }

        public class BlockHitPacket : TickPacket
        {
            private int x;
            private int y;

            private float velX;
            private float velY;

            public void Read(ByteBuf buf)
            {
                this.x = buf.ReadByte();
                this.y = buf.ReadByte();

                this.velX = buf.ReadFloat();
                this.velY = buf.ReadFloat();
            }

            public void Handle()
            {
                Debug.Log("block hit at " + this.x + " " + this.y);
                
                var world = GameManager.instance.gameWorld;
                var block = world.blockManager.GetSpecialBlock(this.x, this.y);
                
                block.PlayHitAnimation(this.velX, this.velY);
                world.blockManager.TickBlock(block);
            }
        }

        public class DamageEntityPacket : TickPacket
        {
            private int entityId;
            private int health;

            public void Read(ByteBuf buf)
            {
                this.entityId = buf.ReadByte();
                this.health = buf.ReadShort();
            }

            public void Handle()
            {
                var world = GameManager.instance.gameWorld;
                var entity = world.GetPlayer(this.entityId);

                if (this.health <= 0)
                {
                    entity.Die();
                }
                else
                {
                    entity.Damage(this.health);
                }
            }
        }

        public class RespawnPlayerPacket : TickPacket
        {
            private int entityId;

            private float x;
            private float y;

            public void Read(ByteBuf buf)
            {
                this.entityId = buf.ReadByte();

                this.x = buf.ReadFloat();
                this.y = buf.ReadFloat();
            }

            public void Handle()
            {
                var world = GameManager.instance.gameWorld;
                var entity = world.GetPlayer(this.entityId);
                
                entity.Respawn(this.x, this.y);
            }
        }

        public class TutorialPacket : TickPacket
        {
            private TutorialDynamicGameData dynamicGameData;

            public void Read(ByteBuf buf)
            {
                this.dynamicGameData = new TutorialDynamicGameData();
                this.dynamicGameData.Read(buf);
            }

            public void Handle()
            {
                var world = GameManager.instance.gameWorld;
                this.dynamicGameData.Handle(world);
            }
        }

        public class GameStatePacket : TickPacket
        {
            private GameState gameState;

            public void Read(ByteBuf buf)
            {
                this.gameState = (GameState)buf.ReadByte();
            }

            public void Handle()
            {
                GameManager.instance.gameWorld.game.UpdateState(this.gameState);
            }
        }

        public class GameStartingPacket : TickPacket
        {
            private int seconds;

            public void Read(ByteBuf buf)
            {
                this.seconds = buf.ReadByte();
            }

            public void Handle()
            {
                GameManager.instance.gameStartUI.SetTime(this.seconds);
            }
        }
        
        public class GameWinnersPacket : TickPacket
        {
            private int[] winners;
            
            public void Read(ByteBuf buf)
            {
                this.winners = new int[buf.ReadByte()];
                for (var i = 0; i < this.winners.Length; i++)
                {
                    this.winners[i] = buf.ReadByte();
                }
            }

            public void Handle()
            {
                var names = new List<string>();
;                
                foreach (var i in this.winners)
                {
                    var p = GameManager.instance.gameWorld.GetPlayer(i);
                    if (p == null) continue;

                    names.Add("player_team_" + p.team);
                }

                GameManager.instance.youDiedUI.SetActive(false);
                GameManager.instance.winnersUI.SetWinners(names);
            }
        }

        public class YouDiedPacket : TickPacket
        {
            public void Read(ByteBuf buf) {}

            public void Handle()
            {
                GameManager.instance.youDiedUI.SetActive(true);
            }
        }
    }
}