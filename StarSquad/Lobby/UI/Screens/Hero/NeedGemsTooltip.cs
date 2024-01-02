using System;
using StarSquad.Common.Level;
using StarSquad.Loader;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Header;
using StarSquad.Lobby.UI.Nav;
using StarSquad.Lobby.UI.Panel;
using StarSquad.Lobby.UI.Tooltip;
using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Screens.Hero
{
    public class NeedGemsTooltip : TooltipBase
    {
        private string originalText;

        public ActiveButton gemButton;
        public TMP_Text gemsText;

        private bool enoughGems;

        private Action<BuyData> onPurchaseDone;

        public class BuyData
        {
            public int useMana;
            public int useCoins;
            public int useGems;
        }

        private readonly BuyData buyData = new();

        public void Init()
        {
            this.originalText = this.text.text;
            this.gemButton.onClick.AddListener(this.HandleGem);
        }

        private void HandleGem()
        {
            if (!this.enoughGems)
            {
                PanelManager.Get().upgrade.Close();
                ScreenManager.GetScreenManager().hero.Hide();
                
                NavManager.Get().ShowGems();
                return;
            }

            this.onPurchaseDone(this.buyData);
        }

        public void SetData(int rarity, UpgradeHeroButton upgradeButton, Action<BuyData> onPurchaseDone)
        {
            this.onPurchaseDone = onPurchaseDone;

            var mana = HeaderManager.Get().GetExp(rarity);
            var coins = HeaderManager.Get().coinsCount;
            var gems = HeaderManager.Get().gemsCount;

            var manaPrice = upgradeButton.manaPrice;
            var coinPrice = upgradeButton.coinPrice;

            var needCoins = coinPrice - coins;
            var needMana = manaPrice - mana;

            var notEnoughMana = needMana > 0;
            var notEnoughCoins = needCoins > 0;

            if (notEnoughMana || notEnoughCoins)
            {
                var needGems = 0;

                var text = "Need ";

                if (notEnoughMana)
                {
                    float manaAsGems;
                    string manaName;
                    switch (rarity)
                    {
                        case 0:
                            manaName = "common";
                            manaAsGems = StaticShopManager.Get().shopData.commonManaAsGems;
                            break;
                        case 1:
                            manaName = "epic";
                            manaAsGems = StaticShopManager.Get().shopData.epicManaAsGems;
                            break;
                        case 2:
                            manaName = "legendary";
                            manaAsGems = StaticShopManager.Get().shopData.legendaryManaAsGems;
                            break;
                        default:
                            manaName = "";
                            manaAsGems = 0f;
                            break;
                    }

                    text += needMana + " more " + manaName + " mana";

                    needGems += Mathf.CeilToInt(manaAsGems * needMana);
                    this.buyData.useMana = manaPrice - needMana;
                }
                else
                {
                    this.buyData.useMana = manaPrice;
                }

                if (notEnoughMana && notEnoughCoins)
                {
                    text += " and ";
                }

                if (notEnoughCoins)
                {
                    text += needCoins + " more coins";

                    needGems += Mathf.CeilToInt(StaticShopManager.Get().shopData.coinAsGems * needCoins);
                    this.buyData.useCoins = coinPrice - needCoins;
                }
                else
                {
                    this.buyData.useCoins = coinPrice;
                }

                text += ".";

                this.buyData.useGems = needGems;

                this.text.text = string.Format(this.originalText, text);

                this.gemsText.text = "" + needGems;
                this.enoughGems = needGems <= gems;
                this.gemsText.color = this.enoughGems ? Color.white : Color.red;

                TooltipManager.Get().Show(this);
                return;
            }

            this.buyData.useMana = manaPrice;
            this.buyData.useCoins = coinPrice;
            this.buyData.useGems = 0;

            TooltipManager.Get().CloseActive();

            this.onPurchaseDone(this.buyData);
        }
    }
}