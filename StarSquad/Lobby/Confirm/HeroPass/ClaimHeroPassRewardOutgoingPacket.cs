using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.HeroPass
{
    public class ClaimHeroPassRewardOutgoingPacket : OutgoingPacket
    {
        private readonly int id;
        private readonly int rewardId;
        private readonly bool hero;

        public ClaimHeroPassRewardOutgoingPacket(int id, int rewardId, bool hero)
        {
            this.id = id;
            this.rewardId = rewardId;
            this.hero = hero;
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteShort((short)this.id);
            buf.WriteByte((byte)this.rewardId);
            buf.WriteBool(this.hero);
        }
    }
}