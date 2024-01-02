using StarSquad.Loader;
using StarSquad.Net.Packet;
using UnityEngine;

namespace StarSquad.Lobby.Confirm
{
    public class LinkGameCenterIncomingPacket : IncomingPacket
    {
        private string name;
        private string tag;
        private string token;

        public void Read(ByteBuf buf)
        {
            this.name = buf.ReadString();
            this.tag = buf.ReadString();
            this.token = buf.ReadString();
        }

        public void Handle()
        {
            Debug.Log("other account found with game center");

            string gameCenter;
#if UNITY_IOS
            gameCenter = "Game Center";
#else
            gameCenter = "Google Play Games";
#endif

            LoaderManager.instance.nativeDialogManager.ShowDialog(
                "Other account found",
                "Found account \"" + this.name + "\" (#" + this.tag + ") linked with current " + gameCenter +
                ". Would you like to load it instead?",
                "Yes", (a) =>
                {
                    PlayerAuthentication.SaveToken(this.token);
                    LoaderManager.instance.Reload();
                });
        }
    }
}