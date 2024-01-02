namespace StarSquad.Game.Block
{
    public class BlockCollision
    {
        public float minX;
        public float minY;
        public float maxX;
        public float maxY;

        public BlockCollision()
        {
            this.minX = 0f;
            this.minY = 0f;
            this.maxX = 1f;
            this.maxY = 1f;
        }

        public BlockCollision(float minX, float minY, float maxX, float maxY)
        {
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }
    }
}