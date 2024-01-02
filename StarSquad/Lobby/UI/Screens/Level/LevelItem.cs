using StarSquad.Game.Misc;
using StarSquad.Lobby.Confirm.Misc;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Reward;
using StarSquad.Lobby.UI.Util;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Screens.Level
{
    public class LevelItem : MonoBehaviour
    {
        public Image fill;
        private Material mat;
        public GameObject progressBar;

        public TMP_Text levelNumText;
        public TMP_Text levelPointsText;

        public GameObject lockedLevelNumBg;
        public GameObject unlockedLevelNumBg;

        public GameObject lockedRewardBg;
        public GameObject unlockedRewardBg;

        public Transform icon;
        public TMP_Text amountText;

        public GameObject claimed;
        public ActiveButton claimButton;

        public void Init(int level, int points, GameObject icon, int amount, bool last)
        {
            this.mat = MaterialUtil.DuplicateMaterial(this.fill);
            this.SetProgress(0f);

            this.levelNumText.text = "" + level;
            this.levelPointsText.text = "" + points;

            Instantiate(icon, this.icon);
            this.amountText.text = "" + amount;

            if (last)
            {
                this.progressBar.SetActive(false);
            }

            this.claimButton.onClick.AddListener(() => this.HandleClaim(level));
        }

        public void SetClaim(bool claim)
        {
            this.claimButton.gameObject.SetActive(claim);
        }

        public void SetClaimed()
        {
            this.claimed.SetActive(true);
        }

        public void SetUnlocked()
        {
            this.lockedLevelNumBg.SetActive(false);
            this.unlockedLevelNumBg.SetActive(true);
            
            this.lockedRewardBg.SetActive(false);
            this.unlockedRewardBg.SetActive(true);
            
            this.levelPointsText.color = Color.white;
        }

        public void SetProgress(float p)
        {
            ProgressBarUtil.SetProgress(this.mat, p);
        }

        public bool IsLocked()
        {
            return this.lockedLevelNumBg.activeSelf;
        }

        private void HandleClaim(int level)
        {
            this.claimButton.SetDisabled();
            
            var id = RewardManager.Get().AddToQueue(this.icon.transform.position, () => this.claimButton.SetEnabled());
            PacketConfirmManager.Get().Send(new ClaimLevelRewardOutgoingPacket(id, level));
        }
    }
}