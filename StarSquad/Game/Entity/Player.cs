using System;
using System.Collections.Generic;
using StarSquad.Game;
using StarSquad.Game.Block.Blocks;
using StarSquad.Game.Hero;
using StarSquad.Game.Misc;
using StarSquad.Net;
using StarSquad.Net.Packet.Game;
using StarSquad.Net.Packet.Game.Hello;
using StarSquad.Net.Udp;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StarSquad.Game.Entity
{
    public class Player : EntityBase
    {
        public float x;
        public float y;
        public float rot;

        public float lastX;
        public float lastY;

        protected float width = 0.4f;

        private GameObject gameObject;
        public HeroBase hero;
        private bool visible = true;

        protected bool running;

        private bool wasInGrass;

        protected Player(GameWorld world, GameDataIncomingPacket.PlayerInfo playerInfo) : base(
            world,
            playerInfo.entityId,
            playerInfo.team,
            playerInfo.maxHealth)
        {
            this.gameObject = new GameObject("player_" + this.entityId);

            this.hero = new HeroGunman(this.gameObject.transform);

            var ring = Object.Instantiate(world.LoadGeneralGameAsset("ring_" + (playerInfo.team ? "green" : "red") + ".prefab"), this.gameObject.transform);
            var blobShadow = Object.Instantiate(world.LoadGeneralGameAsset("blob_shadow.prefab"), ring.transform);
            const float shadowScale = 0.85f;
            blobShadow.transform.localScale = new Vector3(shadowScale, shadowScale, shadowScale);
            AngleUtil.FixAngle(ring.transform, AngleUtil.RingOffset);
        }

        public override void Render(float partialTicks)
        {
            // position
            var renderX = this.lastX + (this.x - this.lastX) * partialTicks;
            var renderY = this.lastY + (this.y - this.lastY) * partialTicks;

            this.gameObject.transform.position = new Vector3(renderX, 0, renderY);

            // rotation
            var toRotate = this.gameObject.transform.GetChild(0);
            toRotate.rotation = Quaternion.Euler(0,
                Mathf.LerpAngle(toRotate.eulerAngles.y, this.rot * Mathf.Rad2Deg,
                    Time.deltaTime * 20f), 0);

            // nametag
            this.nametag.SetPosition(renderX, renderY);

            this.hero.Render(partialTicks);

            base.Render(partialTicks);
        }

        public override void Tick()
        {
            this.hero.SetRunning(this.running);

            if (!this.IsDead())
                this.TickGrass();

            this.nametag.Tick();

            this.hero.Tick();

            base.Tick();
        }

        public override void Die()
        {
            base.Die();

            this.SetVisible(false);
        }

        private void SetVisible(bool visible)
        {
            if (this.visible == visible) return;
            this.visible = visible;

            this.gameObject.SetActive(visible);
            this.nametag.Show(visible);
        }

        public override void Respawn(float x, float y)
        {
            base.Respawn(x, y);

            this.x = x;
            this.y = y;
            this.lastX = x;
            this.lastY = y;

            this.SetVisible(true);
        }

        private void TickGrass()
        {
            if (true) return;
            
            // add new one
            var blockX = Mathf.FloorToInt(this.x);
            var blockY = Mathf.FloorToInt(this.y);

            /*
             * if in team make the grass transparent around player
             */
            if (this.team || this.hero.lastAttack < 20)
            {
                const int border = 2;
                var minX = blockX - border;
                var maxX = blockX + border;
                var minY = blockY - border;
                var maxY = blockY + border;

                for (var x = minX; x <= maxX; x++)
                {
                    for (var y = minY; y <= maxY; y++)
                    {
                        if ((x == minX || x == maxX) && (y == minY || y == maxY)) continue;

                        if (this.world.blockManager.GetSpecialBlock(x, y) is BlockGrass grass)
                        {
                            //this.world.blockManager.SetTransparentGrass(grass);
                            grass.transparent = true;
                            this.world.blockManager.TickBlock(grass);
                        }
                    }
                }
            }

            var inGrass = this.world.blockManager.GetSpecialBlock(blockX, blockY) is BlockGrass;

            if (inGrass)
            {
                /*
                 * if in team make player transparent
                 */
                if (this.team || this.hero.lastAttack < 20 || this.Distance(this.world.thePlayer) <= 3 * 3)
                {
                    this.hero.MakeTransparent();
                }
                else // else just hide player
                {
                    this.SetVisible(false);
                }
            }
            else if (this.wasInGrass)
            {
                if (this.team)
                {
                    this.hero.MakeVisible();
                }
                else
                {
                    this.SetVisible(true);
                }
            }

            this.wasInGrass = inGrass;
        }

        public float Distance(Player target)
        {
            var x = this.x - target.x;
            var y = this.y - target.y;
            return x * x + y * y;
        }

        public void SetDynamicData(int health, float x, float y)
        {
            this.SetDynamicData(health);
            this.x = x;
            this.y = y;
            this.lastX = x;
            this.lastY = y;

            Debug.Log("set dynamic data " + health + " to " + this);

            if (health <= 0)
            {
                this.SetVisible(false);
            }
        }
    }
}