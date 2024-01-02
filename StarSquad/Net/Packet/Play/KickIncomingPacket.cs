using StarSquad.Loader;
using UnityEngine;

namespace StarSquad.Net.Packet.Play
{
    public class KickIncomingPacket : IncomingPacket
    {
        private string reason;

        public void Read(ByteBuf buf)
        {
            this.reason = buf.ReadString();
        }

        public void Handle()
        {
            Debug.Log("You were disconnected for " + this.reason);

            NetworkManager.GetNet().connectionManager.SafeDisconnect("kicked");

            LoaderManager.instance.nativeDialogManager.ShowDialog("You were disconnected!",
                this.reason,
                "Okay",
                button => LoaderManager.instance.Reload());
        }
    }
}