using UnityEngine;

namespace StarSquad.Game.Hero
{
    public class HeroSmasher : HeroBase
    {
        private ParticleSystem groundHitParticle;

        private const int GroundHitParticleIndex = 0;

        public HeroSmasher(Transform transform) : base(transform, 1)
        {
            this.groundHitParticle =
                Object.Instantiate(
                    this.assetBundle.LoadAsset<ParticleSystem>(this.staticHero.particles[GroundHitParticleIndex]));
        }

        public void OnPostAttack(float x, float y, float rot, Vector2 vec)
        {
            var forwardVel = vec * .8f;
            var forwardX = x + forwardVel.x;
            var forwardY = y + forwardVel.y;

            var transform = this.groundHitParticle.transform;
            transform.position = new Vector3(forwardX, 0, forwardY);

            var angles = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(angles.x,
                rot * Mathf.Rad2Deg,
                angles.z);
            this.groundHitParticle.Play();
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override void Render(float partialTicks)
        {
        }
    }
}