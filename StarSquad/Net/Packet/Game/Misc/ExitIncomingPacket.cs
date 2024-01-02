using StarSquad.Game;

namespace StarSquad.Net.Packet.Game.Misc
{
    public class ExitIncomingPacket : IncomingPacket
    {
        public void Read(ByteBuf buf)
        {
        }

        public void Handle()
        {
            GameManager.instance.exitButton.gameObject.SetActive(true);
        }
    }
}