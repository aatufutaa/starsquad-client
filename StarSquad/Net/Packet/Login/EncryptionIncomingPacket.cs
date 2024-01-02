using StarSquad.Loader;
using StarSquad.Net.Session;
using UnityEngine;

namespace StarSquad.Net.Packet.Login
{
    public class EncryptionIncomingPacket : IncomingPacket
    {
        public void Read(ByteBuf buf)
        {
        }

        public void Handle()
        {
            LoaderManager.instance.networkManager.sessionManager.AssertState(LoginState.Encrypt);
            
            Debug.Log("Start encryption");
            NetworkManager.GetNet().connectionManager.EnableEncryption();
            
            PlayerAuthentication.ReadAndSendToken();
        }
    }
}