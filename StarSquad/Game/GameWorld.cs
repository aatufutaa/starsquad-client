using System.Collections.Generic;
using StarSquad.Game.Block;
using StarSquad.Game.Entity;
using StarSquad.Game.Misc;
using StarSquad.Game.Mode;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using StarSquad.Net;
using StarSquad.Net.Packet.Game;
using StarSquad.Net.Packet.Game.Hello;
using StarSquad.Net.Packet.Game.Misc;
using StarSquad.Net.Udp;
using UnityEngine;

namespace StarSquad.Game
{
    public class GameWorld
    {
        public readonly BlockManager blockManager;

        public readonly CameraFollow cameraFollow;

        public ThePlayer thePlayer;

        private List<Projectile> bullets = new List<Projectile>();

        private int currentTick;

        private List<Player> players = new List<Player>();
        private Dictionary<int, EntityBase> entities = new Dictionary<int, EntityBase>();

        public bool staticDataLoaded;
        public bool dynamicDataLoaded;

        private bool attack;
        private Vector2 attackTarget;

        public GameMode game;

        private CustomAssetBundle generalGameAssets;

        public GameWorld()
        {
            this.blockManager = new BlockManager();
            this.cameraFollow = new CameraFollow();
        }

        public void Init(GameType gameType, string name, int mapSizeX, int mapSizeY)
        {
            this.generalGameAssets = AssetManager.LoadAssetBundle0("general_game_bundle");

            switch (gameType)
            {
                case GameType.Tutorial:
                    this.game = new TutorialGame();
                    break;

                case GameType.LastHeroStanding:
                    this.game = new LHSGame();
                    break;

                default:
                    LoaderManager.instance.ShowBadError("This game mode is not implemented");
                    return;
            }

            this.blockManager.Init(name, mapSizeX, mapSizeY);
            this.cameraFollow.Init(mapSizeX, mapSizeY);

            if (!LoaderManager.IsUsingNet())
            {
                GameDataIncomingPacket.PlayerInfo playerInfo = new GameDataIncomingPacket.PlayerInfo();
                playerInfo.entityId = 0;
                playerInfo.maxHealth = 200;
                playerInfo.name = "hello1";
                this.AddPlayer(playerInfo, true);
                this.players[0].SetDynamicData(200);
                this.staticDataLoaded = true;
                this.dynamicDataLoaded = true;
            }

            //this.thePlayer = new Player(this, 1, 200);
            //this.thePlayer.SetDynamicData(200);
        }

        public void AddPlayer(GameDataIncomingPacket.PlayerInfo playerInfo, bool you)
        {
            Player player;
            if (you)
            {
                this.thePlayer = new ThePlayer(this, playerInfo);
                player = this.thePlayer;
            }
            else
            {
                player = new OtherPlayer(this, playerInfo);
            }

            this.players.Add(player);
            this.entities.Add(playerInfo.entityId, player);
        }

        public Player GetPlayer(int entityId)
        {
            return (Player)this.entities[entityId];
        }

        public void HandleMoveInput(Vector2 input)
        {
            if (this.game.gameState != GameState.Started) return;
            this.thePlayer.Walk(input);
        }

        public void HandleAttackInput(Vector2 targetPos)
        {
            if (this.game.gameState != GameState.Started) return;

            this.attack = true;
            this.attackTarget = targetPos;
        }

        public void Tick()
        {
            if (!this.staticDataLoaded) return;
            if (!this.dynamicDataLoaded) return;

            this.game.Tick();

            this.blockManager.Tick();

            //this.thePlayer.Tick();
            foreach (var player in this.players)
            {
                player.Tick();
            }

            this.cameraFollow.SetTarget(this.thePlayer.x, this.thePlayer.y);
            this.cameraFollow.Tick();

            var playerInput = new TickOutgoingPacket.PlayerInput(LoaderManager.IsUsingNet() &&
                                                                 LoaderManager.instance.networkManager.connectionManager
                                                                     .IsFallbackToTcp());

            if (this.attack)
            {
                this.attack = false;

                // play attack animation
                var targetPos = this.attackTarget;
                var direction = new Vector2(targetPos.x - this.thePlayer.x, targetPos.y - this.thePlayer.y).normalized;
                var rot = Mathf.Atan2(direction.x, direction.y);

                if (!this.thePlayer.Attack(direction, rot)) return;

                playerInput.attacked = true;
                playerInput.attackX = this.attackTarget.x;
                playerInput.attackY = this.attackTarget.y;
                playerInput.attackId = this.thePlayer.attackId;

                this.thePlayer.rot = rot;
            }

            if (LoaderManager.IsUsingNet())
            {
                if (this.game.gameState == GameState.Started)
                {
                    playerInput.tick = this.currentTick;
                    playerInput.x = this.thePlayer.x;
                    playerInput.y = this.thePlayer.y;
                    playerInput.lastX = this.thePlayer.lastX;
                    playerInput.lastY = this.thePlayer.lastY;

                    if (!LoaderManager.instance.networkManager.connectionManager.IsFallbackToTcp())
                    {
                        LoaderManager.instance.networkManager.connectionManager.SendUdpPacket(
                            new MoveOutgoingPacket(playerInput));
                    }
                    else
                    {
                        LoaderManager.instance.networkManager.connectionManager.SendPacket(
                            new TickOutgoingPacket(playerInput));
                    }
                }
            }

            ++this.currentTick;
        }

        public void Render(float partialTicks)
        {
            if (!this.staticDataLoaded) return;
            if (!this.dynamicDataLoaded) return;

            this.game.Render();

            this.blockManager.Render();

            //this.thePlayer.Render(partialTicks);
            foreach (var player in this.players)
            {
                player.Render(partialTicks);
            }

            this.cameraFollow.Render(partialTicks);

            foreach (var bullet in this.bullets)
            {
                bullet.Render();
            }
        }

        public void HandleEntityData(List<TickIncomingPacket.EntityData> entities)
        {
            foreach (var entityData in entities)
            {
                var player = (OtherPlayer)this.GetPlayer(entityData.entityId);
                player.SetServerPos(entityData.x, entityData.y, entityData.rot);
            }
        }
        
        public GameObject LoadGeneralGameAsset(string path)
        {
            return this.generalGameAssets.LoadAsset<GameObject>("Assets/Remote/Game/Hero/Access/" + path);
        }
    }
}