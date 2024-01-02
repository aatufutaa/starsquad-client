using System;
using StarSquad.Common.Level;
using StarSquad.Game.Misc;
using StarSquad.Loader;
using StarSquad.Lobby.Confirm.HeroPass;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Reward;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Screens.HeroPass
{
    public class HeroPassItem : MonoBehaviour
    {
        public Transform heroIcon;
        public TMP_Text heroAmount;
        public GameObject lockIcon;
        public ActiveButton heroClaimButton;
        public GameObject heroClaimed;

        public Transform freeIcon;
        public TMP_Text freeAmount;
        public ActiveButton freeClaimButton;
        public GameObject freeClaimed;

        public GameObject firstStep;
        public GameObject lockedStep;
        public GameObject unlockedStep;
        public TMP_Text stepText;

        public CustomButton unlockNextButton;
        public TMP_Text unlockNextText;

        public GameObject progressBar;
        public RectTransform fill;
        private Vector2 originalFillSize;

        private int id;
        [NonSerialized] public int tokens;
        [NonSerialized] public bool unlocked;

        public void Init(int id, StaticHeroPassManager.HeroPassStep data, int tokens)
        {
            this.id = id;
            this.tokens = tokens;

            if (this.id == 0)
            {
                this.firstStep.gameObject.SetActive(true);
                this.stepText.gameObject.SetActive(false);
                this.lockedStep.SetActive(false);
            }
            else
            {
                this.stepText.text = "" + id;
            }

            this.unlockNextButton.onClick.AddListener(this.HandleUnlock);
            this.unlockNextButton.gameObject.SetActive(false);

            var heroReward = data.hero;
            var freeReward = data.free;

            this.heroAmount.text = "" + heroReward.amount;
            this.freeAmount.text = "" + freeReward.amount;

            this.heroClaimButton.onClick.AddListener(this.HandleHeroClaim);
            this.freeClaimButton.onClick.AddListener(this.HandleFreeClaim);

            this.fill.gameObject.SetActive(false);
            this.originalFillSize = this.fill.sizeDelta;
        }

        public void SetData(bool heroPass, int heroClaimIndex, int freeClaimIndex)
        {
            if (heroPass)
            {
                if (this.id < heroClaimIndex)
                {
                    this.heroClaimed.SetActive(true);
                }
                else if (this.id == heroClaimIndex && this.unlocked)
                {
                    this.heroClaimButton.gameObject.SetActive(true);
                }

                this.lockIcon.SetActive(false);
            }

            if (this.id < freeClaimIndex)
            {
                this.freeClaimed.SetActive(true);
            }
            else if (this.id == freeClaimIndex && this.unlocked)
            {
                this.freeClaimButton.gameObject.SetActive(true);
            }
        }

        public void SetUnlocked()
        {
            if (this.lockedStep.activeSelf)
            {
                this.lockedStep.SetActive(false);
                this.unlockedStep.SetActive(true);
            }

            this.fill.gameObject.SetActive(true);
            this.unlocked = true;
        }

        public void OnUnlockNext()
        {
            this.fill.sizeDelta = this.originalFillSize;
            this.unlockNextButton.gameObject.SetActive(false);
        }

        public void SetHalfProgress(int price)
        {
            this.fill.gameObject.SetActive(true);
            this.fill.sizeDelta = new Vector2(this.originalFillSize.x, this.originalFillSize.y / 2);

            this.unlockNextButton.gameObject.SetActive(true);

            this.unlockNextText.text = "" + price;
        }

        private void HandleUnlock()
        {
            ScreenManager.GetScreenManager().heroPass.SetTooltip(this.id, this.unlockNextButton.transform.position);
        }

        private void HandleHeroClaim()
        {
            if (LoaderManager.IsUsingNet())
            {
                var id = RewardManager.Get().AddToQueue(this.heroIcon.position, () => { });
                PacketConfirmManager.Get().Send(new ClaimHeroPassRewardOutgoingPacket(id, this.id, true));
                return;
            }

            ScreenManager.GetScreenManager().heroPass.OnClaimReward(this.id, true);
        }

        private void HandleFreeClaim()
        {
            if (LoaderManager.IsUsingNet())
            {
                var id = RewardManager.Get().AddToQueue(this.freeIcon.position, () => { });
                PacketConfirmManager.Get().Send(new ClaimHeroPassRewardOutgoingPacket(id, this.id, false));
                return;
            }

            ScreenManager.GetScreenManager().heroPass.OnClaimReward(this.id, false);
        }

        public void OnClaim(bool hero)
        {
            if (hero)
            {
                this.heroClaimButton.gameObject.SetActive(false);
                this.heroClaimed.SetActive(true);
            }
            else
            {
                this.freeClaimButton.gameObject.SetActive(false);
                this.freeClaimed.SetActive(true);
            }
        }

        public void SetClaim(bool hero)
        {
            if (hero)
            {
                this.heroClaimButton.gameObject.SetActive(true);
            }
            else
            {
                this.freeClaimButton.gameObject.SetActive(true);
            }
        }

        public int GetRewardCount(bool hasHeroPass)
        {
            var count = 0;

            if (this.unlocked)
            {
                if (hasHeroPass && !this.heroClaimed.activeSelf)
                {
                    ++count;
                }

                if (!this.freeClaimed.activeSelf)
                {
                    ++count;
                }
            }

            return count;
        }
    }
}