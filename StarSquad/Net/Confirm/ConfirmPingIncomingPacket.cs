using StarSquad.Net.Packet;

namespace StarSquad.Net.Confirm
{
    public class ConfirmPingIncomingPacket : IncomingPacket
    {
        private int latestAcceptedId;

        public void Read(ByteBuf buf)
        {
            this.latestAcceptedId = buf.ReadInt();
        }

        public void Handle()
        {
            PacketConfirmManager.Get().ServerConfirm(this.latestAcceptedId);
            NetworkManager.GetNet().connectionManager.SendPacket(new ConfirmPingOutgoingPacket(
                PacketConfirmManager.Get().latestIncomingId
            ));
        }
    }
}