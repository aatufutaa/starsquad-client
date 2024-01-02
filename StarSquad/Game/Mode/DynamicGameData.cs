using StarSquad.Net.Packet;

namespace StarSquad.Game.Mode
{
    public interface DynamicGameData
    {
        public void Read(ByteBuf buf);

        public void Handle(GameWorld world);
    }
}