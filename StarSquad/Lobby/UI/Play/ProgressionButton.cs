using StarSquad.Game.Misc;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Lobby.UI.Screens.Progression;
using StarSquad.Lobby.UI.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Play
{
    public class ProgressionButton : CustomButton
    {
        public TMP_Text text;

        public Image fill;
        private Material mat;

        public NotificationCounter notificationCounter;

        public AfterGameRewards afterGameReward;

        public Transform league;

        private int trophies;

        private int lastIndex;
        private StaticLeagueManager.ProgressionLeague lastLeague;

        public GameObject lastLeagueGameObject;

        private void Awake()
        {
            this.mat = MaterialUtil.DuplicateMaterial(this.fill);
            this.onClick.AddListener(HandleProgression);
        }

        public void SetRewardCount(int count)
        {
            this.notificationCounter.SetCount(count);
        }

        public void AddTrophies(int count)
        {
            this.SetTrophies(this.trophies + count);
        }

        public void SetTrophies(int count)
        {
            this.trophies = count;

            var data = ScreenManager.GetScreenManager().progression.leagueManager.progression;

            var i = this.lastIndex;
            for (; i < data.Count; i++)
            {
                var item = data[i];
                if (count >= item.trophies)
                {
                    if (item.hasLeague)
                    {
                        this.lastLeague = item.league;
                    }

                    continue;
                }

                break;
            }

            this.lastIndex = i - 1;

            var currentData = data[this.lastIndex];
            var currentProgress = count - currentData.trophies;

            int targetProgress;
            float p;
            if (i != data.Count)
            {
                var nextData = data[i];
                targetProgress = nextData.trophies - currentData.trophies;
                p = currentProgress / (float)targetProgress;
            }
            else
            {
                targetProgress = -1;
                p = 1;
            }

            this.text.text = targetProgress == -1 ? "" + count : currentProgress + "/" + targetProgress;
            ProgressBarUtil.SetProgress(this.mat, p);

            if (this.lastLeague != null)
            {
                Destroy(this.lastLeagueGameObject);
                this.lastLeagueGameObject = ScreenManager.GetScreenManager().progression
                    .CreateLeagueIcon(this.lastLeague.league, this.lastLeague.tier, this.league).gameObject;
            }
        }

        private static void HandleProgression()
        {
            ScreenManager.GetScreenManager().progression.Show();
        }
    }
}