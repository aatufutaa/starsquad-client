using StarSquad.Net.Packet;
using UnityEngine;

namespace StarSquad.Game.Block.Blocks
{
    public abstract class BlockBase
    {
        public int x;
        public int y;

        public bool ticking;

        private float hitAnimationTime;
        private Vector2 hitVec;

        private readonly string name;
        protected  GameObject gameObject;

        protected BlockBase(string name)
        {
            this.name = name;
        }

        public virtual void ReadStatic(ByteBuf buf)
        {
            this.x = buf.ReadByte();
            this.y = buf.ReadByte();
        }

        public virtual void ReadDynamic(ByteBuf buf)
        {
        }
        
        protected virtual void AddCollision(BlockManager blockManager)
        {
            blockManager.AddCollision(this.x, this.y, new BlockCollision());
        }
        
        public virtual void Add(BlockManager blockManager)
        {
            this.gameObject = new GameObject(name);
            this.gameObject.transform.position = new Vector3(this.x, 0, this.y);
            
            this.AddCollision(blockManager);
        }

        public void PlayHitAnimation(float velX, float velY)
        {
            this.hitAnimationTime = 0f;
            this.hitVec = new Vector2(velX, velY).normalized * 0.2f;
        }

        public virtual void Render()
        {
            if (!this.ticking) return;

            this.hitAnimationTime += Time.deltaTime;

            const float hitTimer = 0.2f;
            var p = this.hitAnimationTime / hitTimer;

            if (p > 1f)
            {
                this.ticking = false;
                p = 1f;
            }

            if (p > 0.5f)
            {
                p = p - 0.5f;
                p /= 0.5f;
                p = 1f - p;
            }
            else
            {
                p /= 0.5f;
            }

            var velX = this.hitVec.x;
            var velY = this.hitVec.y;

            var x = this.x + velX * p;
            var y = this.y + velY * p;
            
            this.gameObject.transform.position = new Vector3(x, 0, y);
        }

        public virtual void Tick()
        {
        }
    }
}