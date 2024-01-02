using System;
using StarSquad.Game.Misc;
using StarSquad.Net.Packet.Game;
using StarSquad.Net.Packet.Game.Hello;

namespace StarSquad.Game.Entity
{
    public class OtherPlayer : Player
    {
        //private int serverTick;
        private float serverX;
        private float serverY;
        private float serverRot;
        private int smoothTick;

        public OtherPlayer(GameWorld world, GameDataIncomingPacket.PlayerInfo playerInfo) : base(
            world,
            playerInfo
            )
        {
            this.nametag = new Nametag(world, playerInfo.team ? Nametag.NametagColor.Blue : Nametag.NametagColor.Red, this.hero.nametagHeight, playerInfo.name);
        }

        public void SetServerPos(float x, float y, float rot)
        {
            //if (this.serverTick == lastUpdateTick) return;

            //this.serverTick = lastUpdateTick;
            const float moveThreshold = 0.0001f;
            if (Math.Abs(x - this.serverX) > moveThreshold || Math.Abs(y - this.serverY) > moveThreshold ||
                Math.Abs(rot - this.serverRot) > moveThreshold)
            {
                this.serverX = x;
                this.serverY = y;
                this.serverRot = rot;
                this.smoothTick = 3;
            }
        }

        public override void Tick()
        {
            this.lastX = this.x;
            this.lastY = this.y;

            if (this.smoothTick > 0)
            {
                this.x = this.x + (this.serverX - this.x) / this.smoothTick;
                this.y = this.y + (this.serverY - this.y) / this.smoothTick;
                this.rot = this.serverRot;
                this.smoothTick--;
                this.running = true;
            }
            else
            {
                this.running = false;
            }

            base.Tick();
        }

        public override void Respawn(float x, float y)
        {
            base.Respawn(x, y);
            this.smoothTick = 0;
        }
    }
}