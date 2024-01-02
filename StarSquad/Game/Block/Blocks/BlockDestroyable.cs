using StarSquad.Net.Packet;

namespace StarSquad.Game.Block.Blocks
{
    public abstract class BlockDestroyable : BlockBase
    {
        private bool destroyed;
        
        protected BlockDestroyable(string name) : base(name)
        {
        }

        public override void ReadDynamic(ByteBuf buf)
        {
            base.ReadDynamic(buf);
        }
    }
}