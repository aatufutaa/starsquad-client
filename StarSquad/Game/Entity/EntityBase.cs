using System.Runtime.InteropServices;
using StarSquad.Game.Misc;
using UnityEngine;

namespace StarSquad.Game.Entity
{
    public class EntityBase
    {
        protected GameWorld world;

        protected int entityId;
        public bool team;
        private int maxHealth;
        private int health;

        protected Nametag nametag;

        protected EntityBase(GameWorld world, int entityId, bool team, int maxHealth)
        {
            this.world = world;

            this.entityId = entityId;
            this.team = team;
            this.maxHealth = maxHealth;
            this.health = maxHealth;
        }

        public virtual void Tick()
        {
        }

        public virtual void Render(float partialTicks)
        {
            this.nametag.Render();
        }

        public virtual void SetDynamicData(int health)
        {
            this.nametag.Reset();

            this.health = health;

            this.nametag.healthBar.SetHealthAndReset(health, this.maxHealth);
        }

        public virtual void Respawn(float x, float y)
        {
            this.health = this.maxHealth;

            this.nametag.healthBar.SetHealthAndReset(this.health, this.maxHealth);
        }

        public virtual void Die()
        {
            Debug.Log(this.entityId + " died");
        }

        public void Damage(int health)
        {
            this.health = health;
            this.nametag.healthBar.Damage(this.health, this.maxHealth);
        }

        public bool IsDead()
        {
            return this.health <= 0;
        }
    }
}