namespace StarSquad.Net.Packet.Game.Hello
{
    public class StartUdpIncomingPacket : IncomingPacket
    {
        private short udpId;
        
        public void Read(ByteBuf buf)
        {
            this.udpId = buf.ReadShort();
        }

        public void Handle()
        {
           NetworkManager.GetNet().connectionManager.ConnectUdp(this.udpId);
        }
    }
}