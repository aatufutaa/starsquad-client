using StarSquad.Net.Packet;

namespace StarSquad.Net.Confirm
{
    public class ConfirmOutgoingPacket : OutgoingPacket
    {
        private readonly int id;
        private readonly OutgoingPacket packet;

        public ConfirmOutgoingPacket(int id, OutgoingPacket packet)
        {
            this.id = id;
            this.packet = packet;
        }
        
        public void Write(ByteBuf buf)
        {
            buf.WriteInt(this.id);
            
            var id = PacketConfirmManager.Get().packetManager.GetConfirmId(this.packet.GetType());
            buf.WriteByte((byte)id);
            
            this.packet.Write(buf);
        }
    }
}