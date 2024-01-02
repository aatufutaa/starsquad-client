using StarSquad.Loader;
using UnityEngine;

namespace StarSquad.Net.Packet.Lobby.Misc
{
    public class AlertIncomingPacket : IncomingPacket
    {
        private enum AlertType
        {
            FailedToStartGame
        }

        private AlertType alertType;
        
        public void Read(ByteBuf buf)
        {
            this.alertType = (AlertType)buf.ReadByte();
        }

        public void Handle()
        {
            string msg;
            switch (this.alertType)
            {
                case AlertType.FailedToStartGame:
                    msg = "Failed to start a game. Sorry!";
                    break;
                default:
                    Debug.Log("dont know what to alert with " + this.alertType);
                    return;
            }
            
            LoaderManager.instance.alertManager.Alert(msg);
        }
    }
}