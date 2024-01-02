using System;
using System.Diagnostics;
using StarSquad.Common.Level;
using StarSquad.Game.Misc;
using StarSquad.Loader;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Lobby.UI.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Header
{
    public class HeaderManager : MonoBehaviour
    {
        public TMP_Text levelText;
        public TMP_Text levelProgressText;

        public TMP_Text coinsText;
        public TMP_Text gemsText;

        public TMP_Text expCommonText;
        public TMP_Text expRareText;
        public TMP_Text expLegendaryText;

        public TMP_Text expCommonSlideText;
        public TMP_Text expRareSlideText;
        public TMP_Text expLegendarySlideText;

        public Image progressBar;
        private Material fill;

        private int level;
        private int levelPoints;
        private int maxProgress;

        [NonSerialized] public int coinsCount;
        [NonSerialized] public int gemsCount;

        [NonSerialized] public int expCommonCount;
        [NonSerialized] public int expRareCount;
        [NonSerialized] public int expLegendaryCount;

        private void Start()
        {
            this.fill = MaterialUtil.DuplicateMaterial(this.progressBar);

#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                ScreenManager.GetScreenManager().level.SetClaimData(0);
                this.SetLevel(1, 0, StaticLevelManager.Get().levels[1].points);
                this.SetCoins(200);
                this.SetGems(90);
            }
#endif
        }

        public void SetLevel(int level, int currentProgress, int maxProgress)
        {
            this.levelText.text = "" + level;

            var lastLevel = maxProgress == -1;

            this.levelProgressText.text = "" + (lastLevel ? currentProgress : currentProgress + "/" + maxProgress);
            this.level = level;
            this.levelPoints = currentProgress;
            this.maxProgress = maxProgress;

            var p = lastLevel ? 1f : currentProgress / (float)maxProgress;
            ProgressBarUtil.SetProgress(this.fill, p);

            ScreenManager.GetScreenManager().level.UpdateData(level, p);
        }

        public void AddLevel(int levelPoints)
        {
            var level = this.level;
            var newPoints = this.levelPoints + levelPoints;
            var maxProgress = this.maxProgress;

            if (maxProgress != -1)
            {
                var levels = StaticLevelManager.Get().levels;

                var currentLevel = levels[level - 1];
                var nextLevelData = levels[level];
                var nextMaxProgress = nextLevelData.points - currentLevel.points;

                while (newPoints >= nextMaxProgress)
                {
                    newPoints -= nextMaxProgress;
                    ++level;

                    if (level < levels.Count)
                    {
                        var next = levels[level];
                        nextMaxProgress = next.points - nextLevelData.points;
                        nextLevelData = next;
                        maxProgress = nextMaxProgress;
                    }
                    else
                    {
                        maxProgress = -1;
                    }
                }
            }

            this.SetLevel(level, newPoints, maxProgress);
        }

        public void SetCoins(int coins)
        {
            this.coinsCount = coins;
            this.coinsText.text = "" + coins;
        }

        public void AddCoins(int coins)
        {
            this.SetCoins(this.coinsCount + coins);
        }

        public void RemoveCoins(int coins)
        {
            this.SetCoins(this.coinsCount - coins);
        }

        public void SetGems(int gems)
        {
            this.gemsCount = gems;
            this.gemsText.text = "" + gems;
        }

        public void AddGems(int gems)
        {
            this.SetGems(this.gemsCount + gems);
        }

        public void RemoveGems(int amount)
        {
            this.SetGems(this.gemsCount - amount);
        }

        public void SetExpCommon(int amount)
        {
            this.expCommonCount = amount;
            this.expCommonText.text = "" + amount;
            this.expCommonSlideText.text = "" + amount;
        }

        public void AddExpCommon(int amount)
        {
            this.SetExpCommon(this.expCommonCount + amount);
        }

        public void RemoveExpCommon(int amount)
        {
            this.SetExpCommon(this.expCommonCount - amount);
        }

        public void SetExpRare(int amount)
        {
            this.expRareCount = amount;
            this.expRareText.text = "" + amount;
            this.expRareSlideText.text = "" + amount;
        }

        public void AddExpRare(int amount)
        {
            this.SetExpRare(this.expRareCount + amount);
        }

        public void RemoveExpRare(int amount)
        {
            this.SetExpRare(this.expRareCount - amount);
        }

        public void SetExpLegendary(int amount)
        {
            this.expLegendaryCount = amount;
            this.expLegendaryText.text = "" + amount;
            this.expLegendarySlideText.text = "" + amount;
        }

        public void AddExpLegendary(int amount)
        {
            this.SetExpLegendary(this.expLegendaryCount + amount);
        }

        public void RemoveExpLegendary(int amount)
        {
            this.SetExpLegendary(this.expLegendaryCount - amount);
        }

        public int GetExp(int rarity) => rarity switch
        {
            0 => this.expCommonCount,
            1 => this.expRareCount,
            2 => this.expLegendaryCount,
            _ => 0
        };

        public static HeaderManager Get()
        {
            return LobbyManager.instance.headerManager;
        }
    }
}