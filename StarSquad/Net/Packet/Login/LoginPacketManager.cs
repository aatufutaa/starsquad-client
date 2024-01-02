using StarSquad.Loader;
using UnityEngine;

namespace StarSquad.Net.Packet.Login
{
    public class LoginPacketManager : PacketManager
    {
        public LoginPacketManager()
        {
            this.RegisterIncoming(0, typeof(HelloIncomingPacket));
            this.RegisterOutgoing(0, typeof(HelloOutgoingPacket));
            
            this.RegisterIncoming(1, typeof(RequestAccessKeyIncomingPacket));
            this.RegisterOutgoing(1, typeof(RequestAccessKeyOutgoingPacket));

            this.RegisterIncoming(2, typeof(EncryptionIncomingPacket));
            this.RegisterOutgoing(2, typeof(EncryptionOutgoingPacket));

            this.RegisterIncoming(3, typeof(LoginIncomingPacket));
            this.RegisterOutgoing(3, typeof(LoginOutgoingPacket));
        }

        public override void OnConnected()
        {
            var android = Application.platform == RuntimePlatform.Android;
            NetworkManager.GetNet().connectionManager.SendPacket(new HelloOutgoingPacket(LoaderConstants.VersionMajor, LoaderConstants.VersionMinor, android));
        }
    }
}