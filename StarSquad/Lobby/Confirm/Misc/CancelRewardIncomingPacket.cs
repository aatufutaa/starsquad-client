using StarSquad.Loader;
using StarSquad.Lobby.UI.Reward;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Misc
{
    public class CancelRewardIncomingPacket : IncomingPacket
    {
        private int id;
        
        public void Read(ByteBuf buf)
        {
            this.id = buf.ReadShort();
        }

        public void Handle()
        {
            var data = RewardManager.Get().HandleResponse(this.id);
            if (data == null) return;
            data.onCancel();
            LoaderManager.instance.alertManager.Alert("Failed to claim reward!");
        }
    }
}