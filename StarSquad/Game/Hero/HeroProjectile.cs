using StarSquad.Game.Misc;
using UnityEngine;

namespace StarSquad.Game.Hero
{
    public class HeroProjectile : HeroBase
    {
        private Projectile[] projectiles;

        private ParticleSystem gunFireParticle;

        private const int ShootParticleIndex = 0;
        private const int BulletParticleIndex = 1;
        private const int BulletHitParticleIndex = 2;

        public HeroProjectile(Transform transform, int hero) : base(transform, hero, 0.8f)
        {
            this.projectiles = new Projectile[5];
            for (var i = 0; i < this.projectiles.Length; i++)
            {
                this.projectiles[i] = new Projectile(this, this.attackHeight);
            }

            this.gunFireParticle = this.LoadParticle(ShootParticleIndex).GetComponent<ParticleSystem>();
        }

        public override void OnPreAttack(float x, float y, float rot, Vector2 vec)
        {
            base.OnPreAttack(x, y, rot, vec); // attack animation

            var forwardVel = vec * .8f;
            var forwardX = x + forwardVel.x;
            var forwardY = y + forwardVel.y;

            var transform = this.gunFireParticle.transform;
            transform.position = new Vector3(forwardX, 0.8f, forwardY);

            var angles = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(angles.x,
                rot * Mathf.Rad2Deg,
                angles.z);
            this.gunFireParticle.Play();
            // TODO: play particle
            // TODO: play attack sound
        }

        public void OnPostAttack(int projectileId, float x, float y, Vector2 vel)
        {
            this.projectiles[projectileId].Shoot(x, y, vel);
        }

        public void Remove(int projectileId, float x, float y)
        {
            this.projectiles[projectileId].Hit(x, y);
        }

        public override void Tick()
        {
            base.Tick();
            
            foreach (var projectile in this.projectiles)
            {
                projectile.Tick();
            }
        }

        public override void Render(float partialTicks)
        {
            foreach (var projectile in this.projectiles)
            {
                projectile.Render(partialTicks);
            }
        }

        public class Projectile
        {
            private SpriteRenderer bullet;
            private TrailRenderer trail;

            private ParticleSystem hitParticle;

            private Vector2 vel;

            private float x;
            private float y;

            private float lastX;
            private float lastY;

            private float height;

            private int age;
            private bool dead = true;

            private bool first;

            private bool fadingOut;
            private float fadingTimer;
            private Color defaultSpriteColor;
            private Color defaultTrailColor;

            public Projectile(HeroBase hero, float height)
            {
                this.bullet = hero.LoadParticle(BulletParticleIndex).GetComponent<SpriteRenderer>();
                this.trail = this.bullet.transform.GetChild(0).GetComponent<TrailRenderer>();
                AngleUtil.FixAngle(this.bullet.transform, -10);
                this.bullet.gameObject.SetActive(false);
                
                this.hitParticle = hero.LoadParticle(BulletHitParticleIndex).GetComponent<ParticleSystem>();
                AngleUtil.FixAngle(this.hitParticle.transform, -11);

                this.defaultSpriteColor = this.bullet.color;
                this.defaultTrailColor = this.trail.startColor;

                this.height = height;
            }

            public void Shoot(float x, float y, Vector2 vel)
            {
                this.x = x;
                this.y = y;
                this.lastX = x;
                this.lastY = y;
                this.vel = vel;

                this.dead = false;
                this.bullet.gameObject.SetActive(true);

                this.age = 0;

                this.first = true;

                this.fadingOut = false;
                this.bullet.color = this.defaultSpriteColor;
                this.trail.startColor = this.defaultTrailColor;
            }

            public void Hit(float x, float y)
            {
                this.OnDeath();

                this.hitParticle.transform.position = new Vector3(x, this.height, y);
                this.hitParticle.Play();
                // TODO: play hit sound
            }

            private void FadeOut()
            {
                this.fadingOut = true;
                this.fadingTimer = 0f;
            }

            private void OnDeath()
            {
                this.dead = true;
                this.bullet.gameObject.SetActive(false);
            }

            public void Tick()
            {
                if (this.dead) return;

                ++this.age;

                if (this.age > 20)
                {
                    if (true)
                    {
                        if (!this.fadingOut)
                        {
                            this.FadeOut();
                        }
                    }
                    else
                    {
                        this.Hit(this.x, this.y);
                        return;
                    }
                }

                this.lastX = this.x;
                this.lastY = this.y;

                this.x += this.vel.x;
                this.y += this.vel.y;
            }

            public void Render(float partialTicks)
            {
                if (this.dead) return;

                if (this.fadingOut)
                {
                    this.fadingTimer += Time.deltaTime;

                    const float maxFade = 0.2f;
                    var p = this.fadingTimer / maxFade;

                    if (p >= 1f)
                    {
                        this.fadingOut = false;
                        this.OnDeath();
                        p = 1f;
                    }

                    var alpha = 1f - p;

                    this.bullet.color = new Color(
                        this.defaultSpriteColor.r,
                        this.defaultSpriteColor.g,
                        this.defaultSpriteColor.b,
                        alpha);

                    this.trail.startColor = new Color(
                        this.defaultTrailColor.r,
                        this.defaultTrailColor.g,
                        this.defaultTrailColor.b,
                        alpha);
                }

                this.bullet.transform.position = new Vector3(
                    this.lastX + (this.x - this.lastX) * partialTicks,
                    this.height,
                    this.lastY + (this.y - this.lastY) * partialTicks);

                // reset trail so it doesnt teleport from old position
                if (!this.first) return;
                this.first = false;
                this.trail.Clear();
            }
        }
    }
}