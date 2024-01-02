using System;
using StarSquad.Loader;
using StarSquad.Net.Encryption;
using StarSquad.Net.Packet.Login;

namespace StarSquad.Net.Session
{
    public class SessionManager
    {
        private LoginState loginState = LoginState.Hello;

        public Rc4Encryption encryption;

        public string session;

        public bool dataLoaded;

        public string playerId;

        public void AssertState(LoginState state)
        {
            if (this.loginState != state)
            {
                throw new Exception("wrong state is " + this.loginState + " ex " + state);
            }
        }

        public void SetState(LoginState state)
        {
            this.loginState = state;
        }

        public void EnableEncryption()
        {
            this.encryption = new Rc4Encryption();
            
            // encrypt secret created by client and send to server
            var key = RsaEncryption.Encrypt(this.encryption.secret);
            LoaderManager.instance.networkManager.connectionManager.SendPacket(new EncryptionOutgoingPacket(key));

            this.loginState = LoginState.Encrypt;
        }
    }
}