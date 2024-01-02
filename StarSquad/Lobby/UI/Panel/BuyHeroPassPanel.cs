using StarSquad.Lobby.Confirm.HeroPass;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Header;
using StarSquad.Lobby.UI.Nav;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Confirm;
using TMPro;

namespace StarSquad.Lobby.UI.Panel
{
    public class BuyHeroPassPanel : GamePanel
    {
        public TMP_Text buttonText;

        public CustomButton buyButton;

        private void Awake()
        {
            this.buyButton.onClick.AddListener(this.HandleBuy);
            this.buttonText.text = "" + ScreenManager.GetScreenManager().heroPass.heroPassManager.heroPassPrice;
        }

        private void HandleBuy()
        {
            this.Close();
            
            if (HeaderManager.Get().gemsCount < ScreenManager.GetScreenManager().heroPass.heroPassManager.heroPassPrice)
            {
                ScreenManager.GetScreenManager().heroPass.Hide();
                NavManager.Get().ShowGems();
                return;
            }
            
            PacketConfirmManager.Get().Send(new BuyHeroPassOutgoingPacket());
        }
    }
}