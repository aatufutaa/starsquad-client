using StarSquad.Loader;
using StarSquad.Net.Packet;

namespace StarSquad.Net.Confirm
{
    public class FlushConfirmIncomingPacket : IncomingPacket
    {
        private bool send;
        private int latestAcceptedServerId;

        public void Read(ByteBuf buf)
        {
            this.send = buf.ReadBool();
            if (this.send)
                this.latestAcceptedServerId = buf.ReadInt();
        }

        public void Handle()
        {
            if (this.send)
                PacketConfirmManager.Get().OnConnected(this.latestAcceptedServerId);

            LoaderManager.instance.HideLoader();
        }
    }
}