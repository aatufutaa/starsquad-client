using StarSquad.Common.Level;
using StarSquad.Lobby.UI.Reward;
using StarSquad.Lobby.UI.Util;
using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Screens.Level
{
    public class LevelScreen : GameScreen
    {
        public LevelItem prefab;
        public Transform parent;

        public ScrollRect scrollRect;

        private LevelItem[] items;
        
        private int currentClaimIndex;
        
        public void SetClaimData(int claimIndex)
        {
            var levels = StaticLevelManager.Get().levels;

            this.items = new LevelItem[levels.Count];

            for (var i = 0; i < levels.Count; i++)
            {
                var level = levels[i];
                
                var item = Instantiate(this.prefab, this.parent);

                var icon = RewardManager.Get().GetIcon(level.reward.type);

                int points;
                if (i != 0)
                {
                    points = level.points - levels[i - 1].points;
                }
                else
                {
                    points = 0;
                }

                item.Init(i + 1, points, icon, level.reward.amount, i == levels.Count - 1);

                if (i < claimIndex)
                {
                    item.SetClaimed();
                }
                
                this.items[i] = item;
            }

            this.currentClaimIndex = claimIndex;
        }

        public void UpdateData(int level, float p)
        {
            for (var i = 0; i < level; i++)
            {
                var item = this.items[i];
                item.SetUnlocked();
                
                if (i == level - 1)
                {
                    item.SetProgress(p);
                }
                else
                {
                    item.SetProgress(1f);
                }
            }
            
            this.UpdateClaimIndex(this.currentClaimIndex);
        }

        public void UpdateClaimIndex(int index)
        {
            this.currentClaimIndex = index;
            if (index > 0)
            {
                var last = this.items[index - 1];
                last.SetClaim(false);
                last.SetClaimed();
            }

            if (index >= this.items.Length) return;
            var item = this.items[index];
            if (item.IsLocked()) return;
            item.SetClaim(true);
        }

        public override void Show()
        {
            base.Show();
            this.scrollRect.verticalNormalizedPosition = this.currentClaimIndex / (float)this.items.Length;
        }
    }
}