using System;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Header;
using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Screens.Hero
{
    public class UpgradeHeroButton : CustomButton
    {
        public GameObject lockedButton;
        public GameObject unlockedButton;

        public TMP_Text expText;
        public TMP_Text coinsText;

        public GameObject expCommon;
        public GameObject expRare;
        public GameObject expLegendary;

        public TMP_Text buttonText;

        [NonSerialized] public int manaPrice;
        [NonSerialized] public int coinPrice;

        public void SetPrice(bool upgrade, int rarity, int exp, int coins)
        {
            this.buttonText.text = upgrade ? "Upgrade" : "Buy now";

            this.expText.text = "" + exp;
            this.coinsText.text = "" + coins;

            var enoughExp = HeaderManager.Get().GetExp(rarity) >= exp;
            var enoughCoins = HeaderManager.Get().coinsCount >= coins;

            this.expText.color = enoughExp ? Color.white : Color.red;
            this.coinsText.color = enoughCoins ? Color.white : Color.red;

            var enough = enoughExp && enoughCoins;
            this.lockedButton.SetActive(!enough);
            this.unlockedButton.SetActive(enough);

            this.expCommon.SetActive(rarity == 0);
            this.expRare.SetActive(rarity == 1);
            this.expLegendary.SetActive(rarity == 2);

            this.manaPrice = exp;
            this.coinPrice = coins;
        }
    }
}