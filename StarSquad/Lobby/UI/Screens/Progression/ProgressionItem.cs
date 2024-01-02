using System;
using StarSquad.Common.Level;
using StarSquad.Game.Misc;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using StarSquad.Lobby.Confirm.Misc;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Reward;
using StarSquad.Lobby.UI.Util;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace StarSquad.Lobby.UI.Screens.Progression
{
    public class ProgressionItem : MonoBehaviour
    {
        public TMP_Text unlockedText;
        public TMP_Text lockedText;

        public Image fill;
        private Material mat;
        public Image grayFill;
        private Material otherMat;
        public GameObject progressBar;

        public Transform icon;
        public TMP_Text rewardAmountText;
        public ActiveButton claimButton;
        public GameObject claimed;

        public Transform league;

        [NonSerialized] public int id;
        [NonSerialized] public int currentTrophies;

        public void Init(int id, int trophies, int rewardType, int rewardAmount)
        {
            this.id = id;
            this.unlockedText.text = "" + trophies;
            this.lockedText.text = "" + trophies;

            Instantiate(RewardManager.Get().GetIcon(rewardType), this.icon);
            this.rewardAmountText.text = "" + rewardAmount;

            this.claimButton.onClick.AddListener(this.HandleClaim);

            this.currentTrophies = trophies;

            this.mat = MaterialUtil.DuplicateMaterial(this.fill);
            this.otherMat = MaterialUtil.DuplicateMaterial(this.grayFill);
        }

        private void HandleClaim()
        {
#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                ScreenManager.GetScreenManager().progression.OnClaim(this.id);
                return;
            }
#endif

            this.claimButton.SetDisabled();
            var id = RewardManager.Get().AddToQueue(this.icon.position, () => { this.claimButton.SetEnabled(); });
            PacketConfirmManager.Get().Send(new ClaimProgressionOutgoingPacket(id, this.id));
        }

        public void SetClaimed()
        {
            this.claimed.SetActive(true);
        }

        public void SetProgress(float p)
        {
            if (p == 0f)
            {
                this.fill.gameObject.SetActive(false);
                return;
            }

            ProgressBarUtil.SetProgress(this.mat, p);
        }

        public void SetOtherProgress(float p)
        {
            if (p == 0)
            {
                this.grayFill.gameObject.SetActive(false);
                return;
            }

            ProgressBarUtil.SetProgress(this.otherMat, p);
        }

        public void SetClaim(bool enabled)
        {
            this.claimButton.gameObject.SetActive(enabled);
        }

        public void SetUnlocked()
        {
            this.unlockedText.gameObject.SetActive(true);
            this.lockedText.gameObject.SetActive(false);
        }

        public bool IsClaimed()
        {
            return this.claimed.gameObject.activeSelf;
        }

        public void RemoveProgressBar()
        {
            this.progressBar.SetActive(false);
        }

        public bool CanBeClaim()
        {
            return this.claimButton.gameObject.activeSelf;
        }
    }
}