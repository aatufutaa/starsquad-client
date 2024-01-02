using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Hero
{
    public class UpgradeHeroOutgoingPacket : OutgoingPacket
    {
        private readonly int hero;
        private readonly int level;

        private readonly int exp;
        private readonly int coins;
        private readonly int gems;

        public UpgradeHeroOutgoingPacket(int hero, int level, int exp, int coins, int gems)
        {
            this.hero = hero;
            this.level = level;

            this.exp = exp;
            this.coins = coins;
            this.gems = gems;
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteByte((byte)this.hero);
            buf.WriteByte((byte)this.level);

            buf.WriteShort((short)this.exp);
            buf.WriteShort((short)this.coins);
            buf.WriteShort((short)this.gems);
        }
    }
}