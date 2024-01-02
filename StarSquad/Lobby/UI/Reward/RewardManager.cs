using System;
using System.Collections.Generic;
using StarSquad.Lobby.UI.Header;
using UnityEngine;

namespace StarSquad.Lobby.UI.Reward
{
    public class RewardManager : MonoBehaviour
    {
        // 0 coin
        // 1 gem
        // 2 exp common
        // 3 exp rare
        // 4 exp legendary
        // 5 level points
        public CollectorTargetItem[] targets;

        // coins 1
        // gems 1
        // exp common
        // exp rare
        // exp legendary
        public GameObject[] rewardIcons;
        
        private int id;
        private readonly Dictionary<int, QueuedRewardData> queuedRewards = new();

        public class QueuedRewardData
        {
            public Vector3 pos;
            public Action onCancel;

            public QueuedRewardData(Vector3 pos, Action onCancel)
            {
                this.pos = pos;
                this.onCancel = onCancel;
            }
        }
        
        public int AddToQueue(Vector3 pos, Action onCancel)
        {
            var id = this.id++;
            this.queuedRewards.Add(id, new QueuedRewardData(pos, onCancel));
            return id;
        }

        public QueuedRewardData HandleResponse(int id)
        {
            if (this.queuedRewards.TryGetValue(id, out var val))
            {
                this.queuedRewards.Remove(id);
                return val;
            }
            return null;
        }

        public GameObject GetIcon(int type)
        {
            return this.rewardIcons[type];
        }
        
        public void GiveReward(int targetId, int collectorId, int amount, Vector3 pos)
        {
            var target = this.targets[targetId];
            target.OnPreCollect();
            
            var collector = Instantiate(target.collectors[collectorId], this.transform);
            collector.transform.position = pos;
            
            collector.Init(amount, target);
        }

        public void TestCoin()
        {
            this.GiveReward(0, 0, 20, Vector3.zero);
        }
        
        public void TestGem()
        {
            this.GiveReward(1, 0, 20, Vector3.zero);
        }
        
        public void TestExpCommon()
        {
            this.GiveReward(2, 0, 20, Vector3.zero);
        }
        
        public void TestExpRare()
        {
            this.GiveReward(3, 0, 20, Vector3.zero);
        }
        
        public void TextExpLegendary()
        {
            this.GiveReward(4, 0, 20, Vector3.zero);
        }

        public void TestLevel()
        {
            this.GiveReward(5, 0, 20, Vector3.zero);
        }

        public static RewardManager Get()
        {
            return LobbyManager.instance.rewardManager;
        }
    }
}