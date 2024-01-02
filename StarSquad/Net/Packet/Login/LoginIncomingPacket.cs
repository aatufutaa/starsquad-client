using StarSquad.Loader;
using StarSquad.Net.Session;
using UnityEngine;

namespace StarSquad.Net.Packet.Login
{
    public class LoginIncomingPacket : IncomingPacket
    {
        private enum LoginResponse
        {
            Ok,
            SaveToken,
            Banned,
            Fail
        }

        private LoginResponse response;
        private string msg;

        private string playerId;

        private int serverType;
        private string host;
        private int port;

        public void Read(ByteBuf buf)
        {
            this.response = (LoginResponse)buf.ReadByte();
            this.msg = buf.ReadString();

            if (this.response != LoginResponse.Ok) return;

            this.playerId = buf.ReadString();

            this.serverType = buf.ReadByte();
            this.host = buf.ReadString();
            this.port = buf.ReadShort();
        }

        public void Handle()
        {
            NetworkManager.GetNet().sessionManager.AssertState(LoginState.Login);

            switch (this.response)
            {
                case LoginResponse.SaveToken:
                {
                    PlayerAuthentication.SaveTokenOnAccountCreate(this.msg);
                    return;
                }

                case LoginResponse.Ok:
                {
                    Debug.Log("Login session " + this.msg);

                    var sessionManager = NetworkManager.GetNet().sessionManager;
                    sessionManager.session = this.msg; // save session secret
                    sessionManager.playerId = this.playerId;
                    
                    Debug.Log("Connect to server " + this.serverType);
                    Debug.Log("host " + this.host);
                    Debug.Log("port " + this.port);

                    NetworkManager.GetNet().ChangeServer(this.serverType, this.host, this.port);

                    return;
                }

                case LoginResponse.Banned:
                {
                    Debug.Log("You are banned! You are banned for " + this.msg);

                    NetworkManager.GetNet().connectionManager.SafeDisconnect("banned");

                    LoaderManager.instance.nativeDialogManager.ShowDialog("You are banned!", this.msg, "Okay",
                        button => { LoaderManager.instance.Reload(); });
                    return;
                }

                case LoginResponse.Fail:
                {
                    Debug.Log("Login failed! " + this.msg);

                    NetworkManager.GetNet().connectionManager.SafeDisconnect("login failed");

                    LoaderManager.instance.nativeDialogManager.ShowDialog("Login failed!", this.msg, "Try again",
                        button => { LoaderManager.instance.Reload(); });

                    return;
                }
            }
        }
    }
}