namespace StarSquad.Net.Packet.Game.Misc
{
    public class TickOutgoingPacket : OutgoingPacket
    {
        public class PlayerInput
        {
            public bool tcp;
            
            public int tick;
        
            public float x;
            public float y;
            public float lastX;
            public float lastY;
        
            public bool attacked;
            public float attackX;
            public float attackY;
            public int attackId;

            public PlayerInput(bool tcp)
            {
                this.tcp = tcp;
            }
            
            public void Write(ByteBuf buf)
            {
                buf.WriteShort((short)this.tick);
            
                buf.WriteFloat(this.x);
                buf.WriteFloat(this.y);
                if (!this.tcp)
                {
                    buf.WriteFloat(this.lastX);
                    buf.WriteFloat(this.lastY);
                }

                buf.WriteBool(this.attacked);
                if (this.attacked)
                {
                    buf.WriteFloat(this.attackX);
                    buf.WriteFloat(this.attackY);
                    buf.WriteShort((short)this.attackId);
                }
            }
        }

        private readonly PlayerInput playerInput;

        public TickOutgoingPacket(PlayerInput playerInput)
        {
            this.playerInput = playerInput;
        }
        
        public void Write(ByteBuf buf)
        {
            this.playerInput.Write(buf);
        }
    }
}