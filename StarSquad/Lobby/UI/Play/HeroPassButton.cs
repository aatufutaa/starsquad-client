using StarSquad.Game.Misc;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Lobby.UI.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Play
{
    public class HeroPassButton : CustomButton
    {
        public Image fill;
        private Material mat;
        public TMP_Text progressText;
        
        public TMP_Text stepText;

        public NotificationCounter notificationCounter;

        private int lastIndex;

        private int tokens;

        private void Awake()
        {
            this.mat = MaterialUtil.DuplicateMaterial(this.fill);
            this.onClick.AddListener(this.HandleHeroPass);
        }

        public void SetRewardCount(int count)
        {
            this.notificationCounter.SetCount(count);
        }

        private void HandleHeroPass()
        {
            ScreenManager.GetScreenManager().heroPass.Show();
        }

        public void AddTokens(int tokens)
        {
            this.SetData(this.tokens + tokens);
        }

        public void SetData(int tokens)
        {
            this.tokens = tokens;

            var data = ScreenManager.GetScreenManager().heroPass.heroPassManager.rewards;

            var i = this.lastIndex;
            for (; i < data.Count; i++)
            {
                var item = data[i];
                if (tokens >= item.tokens)
                {
                    continue;
                }

                break;
            }

            this.lastIndex = i - 1;

            var step = this.lastIndex + 1;
            if (step >= data.Count)
            {
                this.progressText.text = "Completed";
                step -= 1;
            }
            else
            {
                var currentData = data[this.lastIndex];
                var currentProgress = tokens - currentData.tokens;

                int targetProgress;
                float p;
                if (i != data.Count)
                {
                    var nextData = data[i];
                    targetProgress = nextData.tokens - currentData.tokens;
                    p = currentProgress / (float)targetProgress;
                }
                else
                {
                    targetProgress = -1;
                    p = 1;
                }

                this.progressText.text = currentProgress + "/" + targetProgress;
                ProgressBarUtil.SetProgress(this.mat, p);
            }

            this.stepText.text = "" + step;
        }
    }
}