using UnityEngine;

namespace StarSquad.Net.Packet.Play
{
    public class SendToServerIncomingPacket : IncomingPacket
    {
        private int serverType;
        private string host;
        private int port;
        
        public void Read(ByteBuf buf)
        {
            this.serverType = buf.ReadByte();
            this.host = buf.ReadString();
            this.port = buf.ReadShort();
        }

        public void Handle()
        {
            Debug.Log("Sending to server " + this.host + ":" + this.port + " " + this.serverType);
            
            NetworkManager.GetNet().ChangeServer(this.serverType, this.host, this.port);
        }
    }
}