namespace StarSquad.Game.Block.Blocks
{
    public class BlockWater : BlockBase
    {
        public BlockWater() : base("Water")
        {
        }

        public override void Add(BlockManager blockManager)
        {
            this.AddCollision(blockManager);
            blockManager.tileManager.AddWater(this.x, this.y);
        }
    }
}