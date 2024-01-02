using StarSquad.Game.Misc;
using UnityEngine;

namespace StarSquad.Game.Block.Blocks
{
    public class BlockGrass : BlockDestroyable
    {
        private const float MaxTimer = 0.5f;
        private float timer;

        public bool transparent;

        private SpriteRenderer spriteRenderer;

        public BlockGrass() : base("Grass")
        {
        }

        public override void Add(BlockManager blockManager)
        {
            base.Add(blockManager);

            var grassPrefab = blockManager.LoadBlockAsset("grass.prefab");

            var grassGameObject = Object.Instantiate(grassPrefab, this.gameObject.transform);
            AngleUtil.FixAngle(grassGameObject.transform, 0);

            this.spriteRenderer = grassGameObject.GetComponent<SpriteRenderer>();
        }

        public override void Tick()
        {
            this.transparent = false;
        }

        public override void Render()
        {
            if (!this.ticking) return;

            if (this.transparent)
            {
                this.timer += Time.deltaTime;

                if (this.timer >= MaxTimer)
                {
                    this.timer = MaxTimer;
                }
            }
            else
            {
                this.timer -= Time.deltaTime;

                if (this.timer <= 0f)
                {
                    this.timer = 0;
                    this.ticking = false;
                }
            }

            var p = 1f - this.timer / MaxTimer;
            const float mina = 0.4f;
            p = mina + (1f - mina) * p;

            this.spriteRenderer.color = new Color(1f, 1f, 1f, p);
        }

        protected override void AddCollision(BlockManager blockManager)
        {
        }
    }
}