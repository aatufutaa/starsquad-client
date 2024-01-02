using StarSquad.Lobby.UI.Header;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Misc
{
    public class RemoveCoinsIncomingPacket : IncomingPacket
    {
        private int type;
        private int amount;

        public void Read(ByteBuf buf)
        {
            this.type = buf.ReadByte();
            this.amount = buf.ReadInt();
        }

        public void Handle()
        {
            switch (this.type)
            {
                case 0:
                    HeaderManager.Get().RemoveCoins(this.amount);
                    break;

                case 1:
                    HeaderManager.Get().RemoveGems(this.amount);
                    break;

                case 2:
                    HeaderManager.Get().RemoveExpCommon(this.amount);
                    break;

                case 3:
                    HeaderManager.Get().RemoveExpRare(this.amount);
                    break;

                case 4:
                    HeaderManager.Get().RemoveExpLegendary(this.amount);
                    break;
            }
        }
    }
}