using StarSquad.Game.Misc;
using UnityEngine;

namespace StarSquad.Game.Block.Blocks
{
    public class BlockBox : BlockDestroyable
    {
        public BlockBox() : base("Box")
        {
        }

        public override void Add(BlockManager blockManager)
        {
            base.Add(blockManager);
            
            var prefab = blockManager.LoadBlockAsset("box.prefab");

            var blockGameObject = Object.Instantiate(prefab, this.gameObject.transform);
            AngleUtil.FixAngle(blockGameObject.transform, 0);
        }
    }
}