

using StarSquad.Loader;
using StarSquad.Net.Session;

namespace StarSquad.Net.Packet.Login
{
    public class RequestAccessKeyIncomingPacket : IncomingPacket
    {
        public void Read(ByteBuf buf)
        {
        }

        public void Handle()
        {
            NetworkManager.GetNet().sessionManager.AssertState(LoginState.Hello);
            NetworkManager.GetNet().connectionManager.SendPacket(new RequestAccessKeyOutgoingPacket(LoaderConstants.AccessKey));
        }
    }
}