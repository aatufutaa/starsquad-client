namespace StarSquad.Lobby.UI.Tooltip
{
    public class TooltipManager
    {
        public TooltipBase activeTooltip;

        public void CloseActive(bool close = true)
        {
            if (!this.activeTooltip) return;

            this.activeTooltip.Hide(close);
            this.activeTooltip = null;
        }

        public void Show(TooltipBase tooltip)
        {
            var same = this.activeTooltip == tooltip;

            this.CloseActive();

            if (same) return;

            tooltip.Show();
            this.activeTooltip = tooltip;
        }

        public static TooltipManager Get()
        {
            return LobbyManager.instance.tooltipManager;
        }
    }
}