using StarSquad.Loader;
using StarSquad.Lobby.Confirm.Hero;
using StarSquad.Lobby.Confirm.Misc;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Header;
using StarSquad.Lobby.UI.Nav;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Lobby.UI.Screens.Hero;
using StarSquad.Lobby.UI.Tooltip;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Panel
{
    public class UpgradePanel : GamePanel
    {
        public TMP_Text headerText;
        private string originalHeader;
        public TMP_Text headerLevelText;

        public TMP_Text attackDamageText;
        public TMP_Text upgradeAttackDamageText;

        public TMP_Text healthText;
        public TMP_Text upgradeHealthText;

        public UpgradeHeroButton upgradeButton;

        public NeedGemsTooltip needGemsTooltip;

        private int hero;
        private int level;
        private int rarity;
        
        public override void Init()
        {
            base.Init();
            this.originalHeader = this.headerText.text;

            this.upgradeButton.onClick.AddListener(this.HandleUpgrade);
            
            this.needGemsTooltip.Init();
        }

        public void Init(int hero, int level,
            int attackDamage, int upgradeAttackDamage,
            int health, int upgradeHealth,
            int rarity, int expPrice, int coinPrice)
        {
            this.hero = hero;
            this.level = level;
            this.rarity = rarity;

            this.headerText.text = string.Format(this.originalHeader, level);
            this.headerLevelText.text = "" + level;

            this.attackDamageText.text = "" + attackDamage;
            this.upgradeAttackDamageText.text = "+" + upgradeAttackDamage;

            this.healthText.text = "" + health;
            this.upgradeHealthText.text = "+" + upgradeHealth;

            this.upgradeButton.SetPrice(true, rarity, expPrice, coinPrice);
        }

        private void HandleUpgrade()
        {
            this.needGemsTooltip.SetData(this.rarity, this.upgradeButton, this.FinishUpgrade);
        }

        private void FinishUpgrade(NeedGemsTooltip.BuyData buyData)
        {
            if (LoaderManager.IsUsingNet())
            {
                PacketConfirmManager.Get().Send(new UpgradeHeroOutgoingPacket(this.hero, this.level,
                    buyData.useMana,
                    buyData.useCoins,
                    buyData.useGems));
            }
            else
            {
                this.Close();

                ScreenManager.GetScreenManager().hero.SetLevel(this.hero, this.level, true, true);
            }
        }
    }
}