using System;
using System.Collections.Generic;
using System.IO;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using StarSquad.Net.Packet.Lobby.Hello;
using UnityEngine;

namespace StarSquad.Lobby.UI.Quests
{
    public class QuestsManager : MonoBehaviour
    {
        public QuestItem prefab;
        public Transform parent;

        private Dictionary<int, QuestItem> items = new();

        private static string GetQuestsPath()
        {
#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                return Application.dataPath + "/Editor/TestData/quests.json";
            }
#endif
            return AssetManager.Get().GetFile("quests.json");
        }

        public void Init()
        {
            this.ReadQuests();
        }

        [Serializable]
        public class QuestStep
        {
            public string title;
            public int maxProgress;
            public int rewardType;
            public int rewardAmount;
        }

        [Serializable]
        public class QuestData
        {
            public int id;
            public List<QuestStep> step;
        }

        [Serializable]
        public class QuestsList
        {
            public List<QuestData> quests;
        }

        private void ReadQuests()
        {
            var line = File.ReadAllText(GetQuestsPath());
            var json = JsonUtility.FromJson<QuestsList>(line);

            foreach (var quest in json.quests)
            {
                var item = Instantiate(this.prefab, this.parent);
                item.Init(quest);
                this.items.Add(quest.id, item);
            }
        }

        public void UpdateItem(LobbyDataIncomingPacket.QuestInfo quest)
        {
            if (this.items.TryGetValue(quest.id, out var val))
            {
                val.SetData(quest.amount, quest.claimIndex);
            }
        }
        
        public void OnClaim(int id, int amount, int claimIndex)
        {
            if (this.items.TryGetValue(id, out var val))
            {
                val.SetData(amount, claimIndex);
            }
        }

        public static QuestsManager Get()
        {
            return LobbyManager.instance.questsManager;
        }
    }
}