using System;
using StarSquad.Net.Packet;

namespace StarSquad.Net.Confirm
{
    public class ConfirmIncomingPacket : IncomingPacket
    {
        private int id;
        private IncomingPacket packet;
        
        public void Read(ByteBuf buf)
        {
            this.id = buf.ReadInt();

            var packetId = buf.ReadByte();
            var t = PacketConfirmManager.Get().packetManager.GetConfirmType(packetId);

            this.packet = (IncomingPacket)Activator.CreateInstance(t);
            this.packet.Read(buf);
        }

        public void Handle()
        {
            PacketConfirmManager.Get().Handle(this.id, this.packet);
        }
    }
}