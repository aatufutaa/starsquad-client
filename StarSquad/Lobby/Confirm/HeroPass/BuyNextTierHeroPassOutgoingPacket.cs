using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.HeroPass
{
    public class BuyNextTierHeroPassOutgoingPacket : OutgoingPacket
    {
        private readonly int id;
        private readonly int tokens;
        private readonly int gems;

        public BuyNextTierHeroPassOutgoingPacket(int id, int tokens, int gems)
        {
            this.id = id;
            this.tokens = tokens;
            this.gems = gems;
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteByte((byte)this.id);
            buf.WriteShort((short)this.tokens);
            buf.WriteShort((short)this.gems);
        }
    }
}