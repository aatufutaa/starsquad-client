using System;
using StarSquad.Lobby.UI.Reward;
using UnityEngine;
using UnityEngine.Events;

namespace StarSquad.Lobby.UI.Header
{
    public class CollectorTargetItem : MonoBehaviour
    {
        [Serializable]
        public class ItemCollectEvent : UnityEvent<int>
        {
        }

        public ItemCollectEvent onCollect = new();

        public CoinCollector[] collectors;

        public string sound;

        public virtual void OnPreCollect()
        {
            LobbyManager.instance.audioManager.PlaySound(this.sound);
        }

        public virtual Vector3 GetTargetPos()
        {
            return this.transform.position;
        }
    }
}