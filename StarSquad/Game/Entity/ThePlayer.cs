using StarSquad.Game.Hero;
using StarSquad.Game.Misc;
using StarSquad.Loader;
using StarSquad.Net;
using StarSquad.Net.Packet.Game;
using StarSquad.Net.Packet.Game.Hello;
using UnityEngine;

namespace StarSquad.Game.Entity
{
    public class ThePlayer : Player
    {
        public Vector2 vel;
        public int walkTicks;

        public const int MaxReloads = 4;
        private int reloads;
        private int reloadTimeTicks;
        private int reloadTicks;
        public int attackId;
        private int lastAttack;
        private float currentRot;
        private bool attacking;

        public ThePlayer(GameWorld world, GameDataIncomingPacket.PlayerInfo playerInfo) : base(world, playerInfo)
        {
            this.reloads = MaxReloads;
            this.reloadTimeTicks = 48;

            this.nametag = new Nametag(world, Nametag.NametagColor.Green, this.hero.nametagHeight, playerInfo.name);
        }

        public bool Attack(Vector2 vel, float rot)
        {
            if (this.reloads > 0)
            {
                if (this.lastAttack < 5) return false;

                this.attacking = true;

                --this.reloads;

                this.hero.OnPreAttack(this.x, this.y, rot, vel);

                if (!LoaderManager.IsUsingNet())
                {
                    if (this.hero is HeroProjectile)
                    {
                        vel = vel * 0.5f;
                        ((HeroProjectile)this.hero).OnPostAttack(0, this.x, this.y, vel);
                    }
                    else if (this.hero is HeroSmasher)
                    {
                        ((HeroSmasher)this.hero).OnPostAttack(this.x, this.y, rot, vel);
                    }
                }

                ++this.attackId;

                this.lastAttack = 0;

                return true;
            }

            this.nametag.reloadBar.PlayNoReloadAnimation();

            return false;
        }

        public override void Die()
        {
            base.Die();
            this.walkTicks = 0;
        }

        public override void Tick()
        {
            this.lastX = this.x;
            this.lastY = this.y;

            if (this.walkTicks > 0)
            {
                --this.walkTicks;

                var fromX = this.x;
                var fromY = this.y;

                this.x += this.vel.x;
                this.x = this.CheckCollision(fromX, this.world.blockManager.mapSizeX, true);

                this.y += this.vel.y;
                this.y = this.CheckCollision(fromY, this.world.blockManager.mapSizeY, false);

                this.running = true;
            }
            else
            {
                this.running = false;
            }

            if (this.reloads < MaxReloads)
            {
                ++this.reloadTicks;
                if (this.reloadTicks >= this.reloadTimeTicks)
                {
                    ++this.reloads;
                    this.reloadTicks = 0;
                }
            }

            ++this.lastAttack;
            if (this.attacking && this.lastAttack > 6)
            {
                this.attacking = false;
                //this.hero.OnAttackFinished();
                if (this.walkTicks > 0)
                {
                    this.rot = this.currentRot;
                }
            }

            base.Tick();
        }

        public override void Render(float partialTicks)
        {
            this.nametag.reloadBar.SetProgress(this.reloads, (this.reloadTicks + partialTicks) / this.reloadTimeTicks);

            if (this.walkTargets != null)
                foreach (var walkTarget in this.walkTargets)
                {
                    walkTarget.Render();
                }

            base.Render(partialTicks);
        }

        private float CheckCollision(float from, float mapSize, bool checkX)
        {
            var to = checkX ? this.x : this.y;

            // fix map min
            if (to - this.width < -mapSize / 2)
                return -mapSize / 2 + this.width;

            // fix map max
            if (to + this.width > mapSize / 2)
                return mapSize / 2 - this.width;

            // check for collision
            var minPlayerX = this.x - this.width;
            var maxPlayerX = this.x + this.width;
            var minPlayerY = this.y - this.width;
            var maxPlayerY = this.y + this.width;

            for (var x = Mathf.FloorToInt(minPlayerX); x <= Mathf.FloorToInt(maxPlayerX); x++)
            {
                for (var y = Mathf.FloorToInt(minPlayerY); y <= Mathf.FloorToInt(maxPlayerY); y++)
                {
                    var collision = this.world.blockManager.GetCollision(x, y);

                    if (collision == null) continue;

                    var blockMinX = x + collision.minX;
                    var blockMaxX = x + collision.maxX;
                    var blockMinY = y + collision.minY;
                    var blockMaxY = y + collision.maxY;

                    /*
                     * fixes weird flickering and getting stuck
                     * (prob has to do with corners)
                     */
                    const float fpeFix = 0.00001f;
                    if (maxPlayerX - blockMinX < fpeFix || blockMaxX - minPlayerX < fpeFix)
                        continue; // not colliding x

                    if (maxPlayerY - blockMinY < fpeFix || blockMaxY - minPlayerY < fpeFix)
                        continue; // not colliding y

                    /* fixed with code above to fix getting stuck and flickering */
                    //var collidingHorizontally = maxPlayerX > blockMinX && minPlayerX < blockMaxX;
                    //if (!collidingHorizontally) continue;

                    //var collidingVertically = maxPlayerY > blockMinY && minPlayerY < blockMaxY;
                    //if (!collidingVertically) continue;

                    if (to > from)
                        return (checkX ? blockMinX : blockMinY) - this.width;

                    if (to < from)
                        return (checkX ? blockMaxX : blockMaxY) + this.width;
                }
            }

            return to; // not colliding
        }

        private WalkTarget[] walkTargets;

        public class WalkTarget
        {
            public GameObject gameObject;

            private int stage;
            private float timer;

            private float ticks;

            public WalkTarget(GameObject target, int stage)
            {
                this.gameObject = Object.Instantiate(target);
                this.stage = stage;

                if (stage == 3)
                {
                    this.gameObject.transform.localScale = new Vector3(2, 2, 2);
                }
            }

            public void Reset()
            {
                this.timer = 0f;
            }

            public void SetTicks(float t)
            {
                this.ticks = t;
            }

            public void Render()
            {
                this.timer += Time.deltaTime;

                const float fadeSpeed = 0.1f;
                if (this.timer > this.stage * fadeSpeed)
                {
                    var timer = this.timer - this.stage * fadeSpeed;
                    var p = timer / 0.5f;


                    if (this.stage == 3)
                    {
                        p = 1f;
                        if (this.timer > this.ticks * 0.05f) p = 0;
                    }
                    else
                    {
                        if (p > 0.5f)
                        {
                            p -= 0.5f;
                            p /= 0.5f;
                            p = 1f - p;
                        }
                        else
                        {
                            p /= 0.5f;
                        }
                    }

                    this.gameObject.transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, p);
                }
                else
                {
                    this.gameObject.transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
                }
            }
        }

        public void Walk(Vector2 vec)
        {
            var dist = vec.magnitude;
            const float maxDist = 300;
            if (dist > maxDist)
            {
                dist = maxDist;
            }

            dist /= maxDist;

            const float speed = 0.12f;
            vec = vec.normalized * speed;

            const int minTicks = 10;
            const int maxTicks = 60;
            this.walkTicks = minTicks + (int)((maxTicks - minTicks) * dist);

            this.vel = vec;

            var rot = Mathf.Atan2(vec.x, vec.y);
            if (!this.attacking) this.rot = rot;

            this.currentRot = rot;

            var from = new Vector2(this.x, this.y);
            var travel = vec * this.walkTicks;
            var to = from + travel;

            var targetPrefab = GameObject.Find("WalkDest");
            AngleUtil.FixAngle(targetPrefab.transform, -10);

            if (this.walkTargets == null)
            {
                this.walkTargets = new WalkTarget[4];
                for (var i = 0; i < this.walkTargets.Length; i++)
                {
                    this.walkTargets[i] = new WalkTarget(targetPrefab, i);
                }
            }

            var travelDist = travel.magnitude;
            var walkStep = travelDist / this.walkTargets.Length;

            var ticks = travelDist / speed;

            for (var i = 0; i < this.walkTargets.Length; i++)
            {
                from += vec.normalized * walkStep;

                var target = this.walkTargets[i];

                target.Reset();
                target.gameObject.transform.position = new Vector3(from.x, 0, from.y) + targetPrefab.transform.position;

                if (i == 3)
                {
                    target.SetTicks(ticks);
                }
            }
        }
    }
}