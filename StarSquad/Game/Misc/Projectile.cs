using UnityEngine;

namespace StarSquad.Game.Misc
{
    public class Projectile
    {
        public GameObject bullet;

        public Vector2 vel;

        public void Tick()
        {
        }

        public void Render()
        {
            var to = this.bullet.transform.position + new Vector3(this.vel.x, 0, this.vel.y);
            this.bullet.transform.position =
                Vector3.MoveTowards(this.bullet.transform.position, to, Time.deltaTime * 2);
        }
    }
}