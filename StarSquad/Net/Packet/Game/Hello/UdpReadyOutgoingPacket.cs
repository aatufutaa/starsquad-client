namespace StarSquad.Net.Packet.Game.Hello
{
    public class UdpReadyOutgoingPacket : OutgoingPacket
    {
        public const int Ok = 0;
        public const int FallbackToTcp = 1;
        public const int ReadyToAcceptPackets = 2;

        private readonly int status;

        public UdpReadyOutgoingPacket(int status)
        {
            this.status = status;
        }
        
        public void Write(ByteBuf buf)
        {
            buf.WriteByte((byte)this.status);
        }
    }
}