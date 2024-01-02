using StarSquad.Net.Packet;
using UnityEngine;

namespace StarSquad.Game.Mode
{
    public class TutorialDynamicGameData : DynamicGameData
    {
        private int tutorialStage;

        private bool showSpinner;
        private int spinnerX;
        private int spinnerY;

        public void Read(ByteBuf buf)
        {
            this.tutorialStage = buf.ReadByte();
            this.showSpinner = buf.ReadBool();
            if (this.showSpinner)
            {
                this.spinnerX = buf.ReadByte();
                this.spinnerY = buf.ReadByte();
            }
        }

        public void Handle(GameWorld world)
        {
            var game = (TutorialGame)world.game;
            
            Debug.Log("Tutorial stage " + this.tutorialStage);
            
            game.SetStage(this.tutorialStage);

            if (this.showSpinner)
            {
                game.UpdateSpinner(this.spinnerX, this.spinnerY);
            }
            else
            {
                game.HideSpinner();
            }
        }
    }
}