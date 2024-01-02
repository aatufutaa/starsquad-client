using StarSquad.Game.Misc;
using UnityEngine;

namespace StarSquad.Game.Block.Blocks
{
    public class BlockTree : BlockDestroyable
    {
        public BlockTree() : base("Tree")
        {
        }

        protected override void AddCollision(BlockManager blockManager)
        {
            Debug.Log("add tree at " + this.x + " " + this.y);
            blockManager.AddCollision(this.x, this.y, new BlockCollision()); // center
            
            blockManager.AddCollision(this.x+1, this.y, new BlockCollision());
            blockManager.AddCollision(this.x, this.y+1, new BlockCollision());
            blockManager.AddCollision(this.x+1, this.y+1, new BlockCollision());

            //blockManager.AddCollision(this.x-1, this.y, new BlockCollision()); // left
            //blockManager.AddCollision(this.x+1, this.y, new BlockCollision()); // right
            //blockManager.AddCollision(this.x, this.y+1, new BlockCollision()); // top
            //blockManager.AddCollision(this.x, this.y-1, new BlockCollision()); // bottom
            
            //blockManager.AddCollision(this.x, this.y-1, new BlockCollision(0f, 0.5f, 1f, 1f)); // bottom
        }

        public override void Add(BlockManager blockManager)
        {
            base.Add(blockManager);

            var prefab = blockManager.LoadMapAsset<GameObject>("tree1.prefab");

            var blockGameObject = Object.Instantiate(prefab, this.gameObject.transform);
            AngleUtil.FixAngle(blockGameObject.transform, 0);
        }
    }
}