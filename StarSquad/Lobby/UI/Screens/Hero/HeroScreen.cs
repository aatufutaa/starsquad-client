using System.Collections;
using StarSquad.Common.Hero;
using StarSquad.Loader;
using StarSquad.Lobby.Confirm.Hero;
using StarSquad.Lobby.Hero;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Header;
using StarSquad.Lobby.UI.Heroes;
using StarSquad.Lobby.UI.Nav;
using StarSquad.Lobby.UI.Panel;
using StarSquad.Lobby.UI.Play;
using StarSquad.Lobby.UI.Tooltip;
using StarSquad.Lobby.UI.Util;
using StarSquad.Net;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Screens.Hero
{
    public class HeroScreen : GameScreen
    {
        public TMP_Text nameText;
        public TMP_Text levelText;

        public TMP_Text ratingText;

        public HeroStats attackDamageStats;
        public HeroStats healthStats;

        public ActiveButton selectButton;
        public TMP_Text selectText;
        public TMP_Text selectedText;

        public UpgradeHeroButton upgradeButton;

        public GameObject common;
        public GameObject rare;
        public GameObject legendary;
        
        public TooltipBase tooltip;

        public TooltipBase buyTooltip;
        
        private int lastHero = -1;
        private int currentLevel;

        private int currentRarity;

        private bool useUpgradeButton;

        public NeedGemsTooltip needGemsTooltip;

        public GameObject commonIcon;
        public GameObject epicIcon;
        public GameObject legendaryIcon;
        public TMP_Text manaText;

        //public CustomVideoPlayer customVideoPlayer;

        public void Init()
        {
            this.selectButton.Init();
            this.selectButton.onClick.AddListener(this.HandleSelect);

            //this.upgradeButton.Init();
            this.upgradeButton.onClick.AddListener(this.HandleUpgrade);

            this.needGemsTooltip.Init();

            //this.customVideoPlayer.Init();
        }

        private void HandleSelect()
        {
            if (LobbyManager.instance.lobbyData.selectedHero == this.lastHero)
            {
                LoaderManager.instance.alertManager.Alert("You have already selected this hero");
                return;
            }

            LobbyManager.instance.lobbyData.selectedHero = this.lastHero;

            // TODO: send packet

            this.OnSelect(true);

            //PlayListener.Get().UpdateHero(this.lastHero);
        }

        private void OnSelect(bool selected)
        {
            if (selected)
            {
                this.selectButton.SetDisabled();
            }
            else
            {
                this.selectButton.SetEnabled();
            }

            this.selectText.gameObject.SetActive(!selected);
            this.selectedText.gameObject.SetActive(selected);
        }

        private void HandleUpgrade()
        {
            if (this.useUpgradeButton)
            {
                var panel = PanelManager.Get().upgrade;

                var data = StaticHeroDataManager.Get().GetData(this.lastHero);
                var level = data.levels[this.currentLevel - 1];
                var next = data.levels[this.currentLevel];

                var addAttackDamage = next.attackDamage - level.attackDamage;
                var addHealth = next.health - level.health;

                panel.Init(this.lastHero, this.currentLevel + 1,
                    level.attackDamage, addAttackDamage,
                    level.health, addHealth,
                    data.rarity, next.expPrice, next.coinPrice);
                panel.Show();

                return;
            }

            LobbyManager.instance.tooltipManager.Show(this.buyTooltip);
        }

        public void SetValue(int hero, bool unlocked, int rating, int level)
        {
            var same = this.lastHero == hero;
            this.lastHero = hero;

            this.SetLevel(hero, level, unlocked, false);
            
            var data = StaticHeroDataManager.Get().GetData(hero);

            this.nameText.text = data.displayName;
            this.ratingText.text = unlocked ? "" + rating : "-";

            this.common.SetActive(data.rarity == 0);
            this.rare.SetActive(data.rarity == 1);
            this.legendary.SetActive(data.rarity == 2);

            this.tooltip.text.text = data.desc;

            this.selectButton.gameObject.SetActive(unlocked);

            this.OnSelect(LobbyManager.instance.lobbyData.selectedHero == hero);
            
            HeroPlayer.Get().Play(hero, false);
        }

        private IEnumerator StartSound()
        {
            var timer = 0f;
            var timer2 = 0f;
            while (true)
            {
                timer += Time.deltaTime;
                timer2 += Time.deltaTime;

                if (timer2 > 0.1f)
                {
                    LobbyManager.instance.audioManager.PlaySound("collect");
                    timer2 = 0f;
                }

                if (timer > 1f) break;

                yield return null;
            }
        }

        public void OnConfirm()
        {
            this.needGemsTooltip.SetData(this.currentRarity, this.upgradeButton, this.FinishBuy);
        }

        private void FinishBuy(NeedGemsTooltip.BuyData buyData)
        {
#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                return;
            }
#endif
            PacketConfirmManager.Get()
                .Send(new UpgradeHeroOutgoingPacket(this.lastHero, 0, buyData.useMana, buyData.useCoins,
                    buyData.useGems));
        }

        public void SetLevel(int hero, int level, bool unlocked, bool upgrade)
        {
            var data = StaticHeroDataManager.Get().GetData(hero);

            this.currentRarity = data.rarity;

            this.commonIcon.SetActive(this.currentRarity == 0);
            this.epicIcon.SetActive(this.currentRarity == 1);
            this.legendaryIcon.SetActive(this.currentRarity == 2);
            this.manaText.text = "" + HeaderManager.Get().GetExp(this.currentRarity);

            if (this.lastHero != hero) return;

            this.currentLevel = level;

            this.levelText.text = "" + level;

            var levelData = data.levels[level - 1];

            if (upgrade)
            {
                this.attackDamageStats.Upgrade(levelData.attackDamage);
                this.healthStats.Upgrade(levelData.health);
                LobbyManager.instance.audioManager.PlaySound("upgrade");
                StartCoroutine(this.StartSound());
            }
            else
            {
                this.attackDamageStats.SetAmount(levelData.attackDamage);
                this.healthStats.SetAmount(levelData.health);
            }

            if (unlocked)
            {
                this.useUpgradeButton = true;
                this.upgradeButton.closeTooltip = true;

                if (level < data.levels.Count)
                {
                    var nextLevelData = data.levels[level];
                    this.upgradeButton.SetPrice(true, data.rarity, nextLevelData.expPrice, nextLevelData.coinPrice);
                    this.upgradeButton.gameObject.SetActive(true);
                }
                else
                {
                    this.upgradeButton.gameObject.SetActive(false);
                }
            }
            else
            {
                this.useUpgradeButton = false;
                this.upgradeButton.closeTooltip = false;

                this.upgradeButton.SetPrice(false, data.rarity, levelData.expPrice, levelData.coinPrice);
                this.upgradeButton.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            //this.customVideoPlayer.StopVideo();
            HeroPlayer.Get().Play(LobbyManager.instance.lobbyData.selectedHero, true);
        }
    }
}