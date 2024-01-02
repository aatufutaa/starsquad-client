
using StarSquad.Lobby.UI.Reward;
using StarSquad.Net.Packet;
using UnityEngine;

namespace StarSquad.Lobby.Confirm.Misc
{
    public class RewardIncomingPacket : IncomingPacket
    {
        private int id;
        private int targetId;
        private int collectorId;
        private int amount;

        public void Read(ByteBuf buf)
        {
            this.id = buf.ReadShort();
            this.targetId = buf.ReadByte();
            this.collectorId = buf.ReadByte();
            this.amount = buf.ReadInt();
        }

        public void Handle()
        {
            var rewardManager = RewardManager.Get();

            Vector3 pos;
            var data = rewardManager.HandleResponse(this.id);
            if (data == null)
            {
                pos = Vector3.zero;
            }
            else
            {
                pos = data.pos;
            }
            
            rewardManager.GiveReward(this.targetId, this.collectorId, this.amount, pos);
        }
    }
}