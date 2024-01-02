using System.Linq;
using StarSquad.Common.Level;
using StarSquad.Game.Misc;
using StarSquad.Loader;
using StarSquad.Lobby.Confirm.HeroPass;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Header;
using StarSquad.Lobby.UI.Nav;
using StarSquad.Lobby.UI.Panel;
using StarSquad.Lobby.UI.Play;
using StarSquad.Lobby.UI.Tooltip;
using StarSquad.Lobby.UI.Util;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Screens.HeroPass
{
    public class HeroPassScreen : GameScreen
    {
        public Transform parent;

        public HeroPassItem prefab;

        public TMP_Text progressText;
        public TMP_Text stepText;
        public Image fill;
        private Material mat;

        public CustomButton activateButton;
        public GameObject activatedCheckmark;

        private HeroPassItem[] items;

        private int heroClaimIndex;
        private int freeClaimIndex;
        private bool heroPass;
        private int unlockIndex;
        private int tokens;

        public TMP_Text seasonEndTimeText;
        private string originalSeasonEndTimeText;
        private float seasonEndTime;

        private const int Minute = 60;
        private const int Hour = Minute * 60;
        private const int Day = Hour * 24;
        private int lastSeconds;

        public TMP_Text buyNextText;
        private string originalBuyText;
        public TMP_Text buyNextButtonText;
        public TooltipBase buyNextTooltip;
        public Transform buyNextTooltipPos;
        public CustomButton buyNextButton;

        private int buyTokens;
        private int useGems;
        private bool enoughGems;
        private int buyTierIndex;

        public StaticHeroPassManager heroPassManager;

        public HeroPassButton heroPassButton;
        
        public void Init()
        {
            this.heroPassManager = new StaticHeroPassManager();
            this.heroPassManager.Load();

            this.mat = MaterialUtil.DuplicateMaterial(this.fill);

            this.items = new HeroPassItem[this.heroPassManager.rewards.Count];
            for (var i = 0; i < this.heroPassManager.rewards.Count; i++)
            {
                var data = this.heroPassManager.rewards[i];

                var item = Instantiate(this.prefab, this.parent);
                item.Init(i, data, data.tokens);

                this.items[i] = item;
            }

            this.items.First().SetUnlocked();
            this.items.Last().progressBar.SetActive(false);

#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                this.SetData(100000, 0, 0, 0, false);
            }
#endif

            this.activateButton.onClick.AddListener(this.HandleActivate);

            this.originalSeasonEndTimeText = this.seasonEndTimeText.text;

            this.originalBuyText = this.buyNextText.text;
            this.buyNextButton.onClick.AddListener(this.HandleBuy);
        }

        public void SetData(int timeLeft, int tokens, int heroClaimIndex, int freeClaimIndex, bool heroPass)
        {
            this.tokens = tokens;
            this.heroClaimIndex = heroClaimIndex;
            this.freeClaimIndex = freeClaimIndex;
            this.heroPass = heroPass;
            
            for (var i = 0; i < this.items.Length; i++)
            {
                var item = this.items[i];

                if (i != this.items.Length - 1)
                {
                    var next = this.items[i + 1];

                    if (tokens >= next.tokens)
                    {
                        next.SetUnlocked();
                        this.unlockIndex = i;
                        Debug.Log("set unlocked " + i);
                    }
                    else if (item.unlocked)
                    {
                        Debug.Log("set half to " + i);
                        this.unlockIndex = i;
                        item.SetHalfProgress(this.GetGemPrice(next.tokens));
                    }
                    else
                    {
                        Debug.Log("skip " + i + " " + tokens + " " + next.tokens);
                    }
                }
                else if (tokens >= item.tokens)
                {
                    item.SetUnlocked();
                    this.unlockIndex = i;
                }

                item.SetData(heroPass, heroClaimIndex, freeClaimIndex);
            }
            
            Debug.Log("after asdasdasda  " + this.unlockIndex);
            
            this.UpdateProgress();

            if (heroPass)
            {
                this.SetHeroPassActivated();
            }
            
            this.seasonEndTime = Time.time + timeLeft;
            
            this.UpdateRewardAmount();
        }

        private void Update()
        {
            var timeLeft = (int)(this.seasonEndTime - Time.time);

            if (timeLeft == this.lastSeconds) return;
            this.lastSeconds = timeLeft;

            if (timeLeft < 0) timeLeft = 0;

            var msg = "";
            var days = timeLeft / Day;
            if (days > 0)
            {
                msg += days + "d";
                timeLeft -= days * Day;
            }

            var hours = timeLeft / Hour;
            if (hours > 0)
            {
                if (msg.Length > 0) msg += " ";
                msg += hours + "h";
                timeLeft -= hours * Hour;
            }

            if (days == 0)
            {
                var mins = timeLeft / Minute;
                if (mins > 0)
                {
                    if (msg.Length > 0) msg += " ";
                    msg += mins + "m";
                    timeLeft -= mins * Minute;
                }

                if (timeLeft > 0)
                {
                    if (msg.Length > 0) msg += " ";
                    msg += timeLeft + "s";
                }
            }

            this.seasonEndTimeText.text = string.Format(this.originalSeasonEndTimeText, msg);
        }

        private void UpdateProgress()
        {
            Debug.Log("UPD " + this.unlockIndex);
            
            var index = this.unlockIndex;
            float p;
            if (index >= this.items.Length - 1)
            {
                index = this.items.Length - 1;
                this.progressText.text = "Completed";
                p = 1f;
            }
            else
            {
                var item = this.items[index];
                var nextItem = this.items[index + 1];
                
                var currentProgress = this.tokens - item.tokens;
                var progress = nextItem.tokens - item.tokens;
                this.progressText.text = currentProgress + "/" + progress;
                p = currentProgress / (float)progress;
            }

            ProgressBarUtil.SetProgress(this.mat, p);

            this.stepText.text = "" + (index+1);
        }

        public void OnClaimReward(int id, bool hero)
        {
            var item = this.items[id];
            item.OnClaim(hero);
            if (hero) this.heroClaimIndex = id + 1;
            else this.freeClaimIndex = id + 1;

            if (id != this.items.Length - 1)
            {
                var next = this.items[id + 1];
                if (next.unlocked)
                {
                    next.SetClaim(hero);
                }
            }
            
            this.UpdateRewardAmount();
        }

        public void OnUnlockNext(int id)
        {
            var item = this.items[id];
            item.OnUnlockNext();

            var next = this.items[id + 1];
            next.SetUnlocked();
            next.SetData(this.heroPass, this.heroClaimIndex, this.freeClaimIndex);
            this.tokens = next.tokens;
            if (id + 1 != this.items.Length - 1)
            {
                next.SetHalfProgress(this.GetGemPrice(this.items[id + 2].tokens));
            }

            this.unlockIndex = id + 1;
            this.UpdateProgress();
            
            this.UpdateRewardAmount();
        }

        private void HandleActivate()
        {
            PanelManager.Get().buyHero.Show();
            //this.OnHeroPassPurchase();
        }

        private void SetHeroPassActivated()
        {
            this.activateButton.gameObject.SetActive(false);
            this.activatedCheckmark.SetActive(true);
        }

        public void OnHeroPassPurchase()
        {
            this.SetHeroPassActivated();

            this.heroPass = true;
            foreach (var item in this.items)
            {
                item.SetData(this.heroPass, this.heroClaimIndex, this.freeClaimIndex);
            }
            
            this.UpdateRewardAmount();
        }

        private int GetGemPrice(int nextTokens)
        {
            Debug.Log("as gems " + StaticShopManager.Get().shopData.heroTokenAsGems);
            Debug.Log("need token " + (nextTokens - this.tokens));
            return Mathf.CeilToInt((nextTokens - this.tokens) * StaticShopManager.Get().shopData.heroTokenAsGems);
        }

        public void SetTooltip(int id, Vector3 pos)
        {
            var next = this.items[id + 1];

            var neededTokens = next.tokens;
            this.buyTokens = neededTokens - this.tokens;
            this.useGems = this.GetGemPrice(neededTokens);
            this.enoughGems = HeaderManager.Get().gemsCount >= this.useGems;
            this.buyTierIndex = id;

            this.buyNextText.text = string.Format(this.originalBuyText, this.buyTokens);
            this.buyNextButtonText.text = "" + this.useGems;
            this.buyNextButtonText.color = this.enoughGems ? Color.white : Color.red;

            this.buyNextTooltipPos.position = pos;
            LobbyManager.instance.tooltipManager.Show(this.buyNextTooltip);
        }

        private void HandleBuy()
        {
            if (!this.enoughGems)
            {
                this.Hide();
                NavManager.Get().ShowGems();
                return;
            }

            if (LoaderManager.IsUsingNet())
            {
                PacketConfirmManager.Get()
                    .Send(new BuyNextTierHeroPassOutgoingPacket(this.buyTierIndex, this.buyTokens, this.useGems));
                return;
            }

            this.OnUnlockNext(this.unlockIndex);
        }

        private void UpdateRewardAmount()
        {
            var count = 0;
            foreach (var item in this.items)
            {
                count += item.GetRewardCount(this.heroPass);
            }
            Debug.Log("reward " + count);
            this.heroPassButton.SetRewardCount(count);
        }
    }
}