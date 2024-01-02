using UnityEngine;

namespace StarSquad.Lobby.UI.Panel
{
    public class PanelManager : MonoBehaviour
    {
        public UpgradePanel upgrade;
        public BuyHeroPassPanel buyHero;

        private void Awake()
        {
            this.upgrade.Init();
        }

        public static PanelManager Get()
        {
            return LobbyManager.instance.panelManager;
        }
    }
}