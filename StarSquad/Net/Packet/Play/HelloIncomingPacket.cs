using StarSquad.Loader;
using StarSquad.Net.Confirm;
using UnityEngine;

namespace StarSquad.Net.Packet.Play
{
    public class HelloIncomingPacket : IncomingPacket
    {
        private const int Ok = 0;
        private const int Fail = 1;

        private int response;

        public void Read(ByteBuf buf)
        {
            this.response = buf.ReadByte();
        }

        public void Handle()
        {
            switch (this.response)
            {
                case Ok:
                    Debug.Log("Hello Ok");

                    NetworkManager.GetNet().connectionManager.EnableEncryption(); // enable encryption

                    if (NetworkManager.GetNet().sessionManager.dataLoaded)
                    {
                        NetworkManager.GetNet().connectionManager
                            .SendPacket(
                                new DataLoadedOutgoingPacket(PacketConfirmManager.Get()
                                    .latestIncomingId)); // already loaded
                    }
                    else
                    {
                        NetworkManager.GetNet().connectionManager
                            .SendPacket(new RequestDataOutgoingPacket()); // request data
                    }

                    LoaderManager.instance.UpdateStage(4);
                    break;

                case Fail:
                    Debug.Log("Hello fail");

                    NetworkManager.GetNet().connectionManager.SafeDisconnect("hello failed");
                    
                    LoaderManager.instance.nativeDialogManager.ShowDialog("Server Error",
                        NetworkManager.GetNet().sessionManager.dataLoaded ? "Lost connection with server and login failed. Sorry!" 
                            : "Could not login to server. Sorry!",
                        "Try again",
                        button => LoaderManager.instance.Reload());
                    break;
            }
        }
    }
}