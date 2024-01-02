using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StarSquad.Loader;
using StarSquad.Lobby.UI.Play;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Screens.Progression
{
    public class ProgressionScreen : GameScreen
    {
        public ProgressionItem prefab;
        public Transform parent;

        public LeagueIcon leagueIconPrefab;

        public StaticLeagueManager leagueManager;

        private Dictionary<int, ProgressionItem> idToItem = new();
        private ProgressionItem[] items;

        public RectTransform content;
        public ScrollRect scrollRect;

        [Serializable]
        public class LeagueItem
        {
            public Sprite sprite;
            public Color color;
        }

        public LeagueItem[] leagueItems;

        public ProgressionButton progression;

        private int firstClaimIndex;

        private int currentTrophies;

        private int rewardCount;
        
#if UNITY_EDITOR
        private const int TotalTrophies = 200;
        private const int HighestTrophies = 500;
#endif

        public TMP_Text trophiesText;

        public void Init()
        {
            // activate gameobject and then disable
            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
                StartCoroutine(this.DisableAfter());
            }

            this.leagueManager = new StaticLeagueManager();
            this.leagueManager.Load();

            this.Load();
        }

        private IEnumerator DisableAfter()
        {
            yield return new WaitForEndOfFrame();

            this.gameObject.SetActive(false);
        }

        public override void Show()
        {
            base.Show();

            var p = this.FindMinClaim();
            this.scrollRect.verticalNormalizedPosition = p;
        }

        public LeagueIcon CreateLeagueIcon(int id, int tier, Transform parent)
        {
            var leagueItem = this.leagueItems[id];
            var createdLeagueItem = Instantiate(this.leagueIconPrefab, parent);
            createdLeagueItem.image.sprite = leagueItem.sprite;
            createdLeagueItem.image.SetNativeSize();
            var tierText = this.leagueManager.tiers[tier];
            createdLeagueItem.text.text = tierText;
            createdLeagueItem.text.color = leagueItem.color;
            createdLeagueItem.shadow.text = tierText;
            return createdLeagueItem;
        }
        
        private void Load()
        {
            this.items = new ProgressionItem[this.leagueManager.progression.Count];
            for (var i = 0; i < this.items.Length; i++)
            {
                var data = this.leagueManager.progression[i];

                var item = Instantiate(this.prefab, this.parent);
                item.Init(data.id, data.trophies, data.reward.type, data.reward.amount);

                if (data.hasLeague)
                {
                    this.CreateLeagueIcon(data.league.league, data.league.tier, item.league);

                    var rc = item.transform.GetComponent<RectTransform>();
                    rc.sizeDelta = new Vector2(rc.sizeDelta.x, rc.sizeDelta.y + 130); // add some space for league
                }

                this.items[i] = item;
                this.idToItem.Add(data.id, item);
            }

            this.items.First().SetUnlocked();
            this.items.Last().RemoveProgressBar();

#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                var claimedRewards = new HashSet<int>();
                claimedRewards.Add(0);
                this.SetData(TotalTrophies, HighestTrophies, claimedRewards);
            }
#endif
        }

        public void SetData(int trophies, int highestTrophies, HashSet<int> claimedRewards)
        {
            this.currentTrophies = trophies;
            this.trophiesText.text = "" + trophies;
            
            var check = true;
            for (var i = 0; i < this.items.Length; i++)
            {
                var item = this.items[i];

                if (i > 0)
                {
                    var lastItem = this.items[i - 1];

                    if (item.currentTrophies <= trophies)
                    {
                        lastItem.SetProgress(1f);
                        lastItem.SetOtherProgress(0f);
                        item.SetUnlocked();
                    }
                    else
                    {
                        var lastTrophies = lastItem.currentTrophies;

                        if (highestTrophies > trophies && highestTrophies >= lastTrophies)
                        {
                            var current = highestTrophies - lastTrophies;
                            var target = item.currentTrophies - lastTrophies;
                            var p = current / (float)target;
                            lastItem.SetOtherProgress(p);
                        }
                        else
                        {
                            lastItem.SetOtherProgress(0f);
                        }

                        if (trophies >= lastTrophies)
                        {
                            var current = trophies - lastTrophies;
                            var target = item.currentTrophies - lastTrophies;
                            var p = current / (float)target;
                            lastItem.SetProgress(p);
                        }
                        else
                        {
                            lastItem.SetProgress(0f);
                        }
                    }
                }

                if (claimedRewards.Contains(item.id))
                {
                    item.SetClaimed();
                }
                else if (item.currentTrophies <= trophies) // only can claim if trophies are enough
                {
                    item.SetClaim(true);
                }

                if (check && trophies >= item.currentTrophies)
                {
                    this.firstClaimIndex = i;

                    if (!item.IsClaimed()) check = false;
                }

                if (item.CanBeClaim() && !item.IsClaimed())
                    ++this.rewardCount;
            }

            //this.UpdateButton(trophies);
            this.UpdateRewardsCount();
        }

        private void UpdateRewardsCount()
        {
            this.progression.SetRewardCount(this.rewardCount);
        }
        
        private void UpdateButton(int trophies)
        {
            /*var currentTrophies = 0;
            var nextTrophies = 0;

            for (var i = this.firstClaimIndex; i < this.items.Length; i++)
            {
                var item = this.items[i];

                Debug.Log("first " + i + " " + item.currentTrophies + " " + trophies);
                if (item.currentTrophies > trophies) break;

                this.firstClaimIndex = i;

                currentTrophies = item.currentTrophies;
                nextTrophies = i != this.items.Length - 1 ? this.items[i + 1].currentTrophies : -1;

                if (item.CanBeClaim())
                {
                    break;
                }
            }

            Debug.Log(currentTrophies);
            Debug.Log(nextTrophies);

            var progress = trophies - currentTrophies;
            var targetProgress = nextTrophies == -1 ? -1 : nextTrophies - currentTrophies;
            this.progression.SetTrophies(progress, targetProgress);*/
        }

        public void OnClaim(int id)
        {
            if (!this.idToItem.TryGetValue(id, out var val)) return;

            val.SetClaim(false);
            val.SetClaimed();

            --this.rewardCount;
            this.UpdateRewardsCount();
            //this.UpdateButton(this.currentTrophies);
        }

        private float FindMinClaim()
        {
            foreach (var item in this.items)
            {
                if (item.CanBeClaim())
                {
                    var pos = item.transform.localPosition.y * -1;
                    var y = this.content.sizeDelta.y;
                    pos = y - pos;

                    var p = pos / y;

                    return p;
                }
            }

            return 1f;
        }
    }
}