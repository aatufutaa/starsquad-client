using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Shop
{
    public class BuyShopItemOutgoingPacket : OutgoingPacket
    {
        private readonly int item;
        private readonly int id;

        public BuyShopItemOutgoingPacket(int item, int id)
        {
            this.item = item;
            this.id = id;
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteShort((short)this.item);
            buf.WriteByte((byte)this.id);
        }
    }
}