using System;
using StarSquad.Game.Misc;
using StarSquad.Lobby.Confirm.Quests;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Reward;
using StarSquad.Lobby.UI.Util;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace StarSquad.Lobby.UI.Quests
{
    public class QuestItem : MonoBehaviour
    {
        public GameObject locked;
        public GameObject unlocked;

        public TMP_Text title;
        public Image fill;
        public TMP_Text progressText;
        private Material mat;

        public Transform icon;
        public TMP_Text amount;
        public ActiveButton claimButton;
        public GameObject claimedCheckmark;

        private QuestsManager.QuestData questData;

        private int currentIndex;

        private GameObject currentIcon;

        public void Init(QuestsManager.QuestData questData)
        {
            this.mat = MaterialUtil.DuplicateMaterial(this.fill);

            this.questData = questData;

            this.claimButton.onClick.AddListener(this.HandleClaim);

            this.SetData(0, 0);
        }

        private void HandleClaim()
        {
            PacketConfirmManager.Get().Send(new QuestOutgoingPacket(this.questData.id, this.currentIndex));
        }

        public void SetData(int amount, int claimIndex)
        {
            var lastStep = claimIndex >= this.questData.step.Count;
            var step = this.questData.step[Math.Min(claimIndex, this.questData.step.Count - 1)];
            this.currentIndex = claimIndex;

            this.title.text = step.title;

            if (this.currentIcon) Destroy(this.currentIcon);
            this.currentIcon = Instantiate(RewardManager.Get().rewardIcons[step.rewardType], this.icon).gameObject;
            this.amount.text = "" + step.rewardAmount;

            var canClaim = amount >= step.maxProgress;
            this.progressText.text = lastStep ? "Completed" : amount + "/" + step.maxProgress;
            var p = lastStep ? 1f : amount / (float)step.maxProgress;
            ProgressBarUtil.SetProgress(this.mat, p);

            this.claimButton.gameObject.SetActive(!lastStep && canClaim);
            this.claimedCheckmark.gameObject.SetActive(lastStep);
        }
    }
}