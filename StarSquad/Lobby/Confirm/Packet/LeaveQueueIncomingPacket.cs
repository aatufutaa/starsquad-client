using StarSquad.Lobby.UI;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Packet
{
    public class LeaveQueueIncomingPacket : IncomingPacket
    {
        private bool failed;
        
        public void Read(ByteBuf buf)
        {
            this.failed = buf.ReadBool();
        }

        public void Handle()
        {
            var queue = ScreenManager.GetScreenManager().queue;
            
            queue.ResetLeaveButton();

            if (!this.failed)
            {
                queue.OnLeave();
            }
        }
    }
}