using StarSquad.Lobby.UI.Button;
using UnityEngine;

namespace StarSquad.Lobby.UI.Tooltip
{
    public class TooltipButton : CustomButton
    {
        public TooltipBase tooltip;

        protected override void HandleClick()
        {
            this.PlayClickSound();
            TooltipManager.Get().Show(this.tooltip);
        }
    }
}