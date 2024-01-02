using StarSquad.Lobby.UI.Header;
using StarSquad.Lobby.UI.Reward;
using UnityEngine;

namespace StarSquad.Lobby.UI.Play
{
    public class AfterGameRewards : MonoBehaviour
    {
        public CollectorTargetItem trophyTarget;
        public CollectorTargetItem tokenTarget;

        public CoinCollector trophyCollector1; // 3
        public CoinCollector trophyCollector2; // 8
        public CoinCollector trophyCollector3; // 13

        public CoinCollector tokenCollector1; // 3
        public CoinCollector tokenCollector2; // 6
        public CoinCollector tokenCollector3; // 12

        public void GiveTrophies(int count)
        {
            var collector = this.trophyCollector1;
            if (count >= 13)
            {
                collector = this.trophyCollector3;
            }
            else if (count >= 8)
            {
                collector = this.trophyCollector2;
            }

            var createdCollector = Instantiate(collector, this.transform);
            createdCollector.Init(count, this.trophyTarget);
        }

        public void GiveTokens(int count)
        {
            var collector = this.tokenCollector1;
            if (count >= 12)
            {
                collector = this.tokenCollector3;
            }
            else if (count >= 6)
            {
                collector = this.tokenCollector2;
            }
            
            var createdCollector = Instantiate(collector, this.transform);
            createdCollector.Init(count, this.tokenTarget);
        }
    }
}