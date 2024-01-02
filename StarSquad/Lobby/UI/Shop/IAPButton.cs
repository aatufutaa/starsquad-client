using StarSquad.Loader;
using StarSquad.Lobby.Confirm.Shop;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Reward;
using StarSquad.Net.Confirm;

namespace StarSquad.Lobby.UI.Shop
{
    public class IAPButton : CustomButton
    {
        public int target;
        public int collect;
        public int amount;

        private void Awake()
        {
            this.onClick.AddListener(this.HandlePurchase);
        }

        private void HandlePurchase()
        {
            var id = RewardManager.Get().AddToQueue(this.transform.position, (() => { }));

            if (LoaderManager.IsUsingNet())
            {
                PacketConfirmManager.Get().Send(new BuyShopItemOutgoingPacket(id, this.collect));
                return;
            }

            var data = RewardManager.Get().HandleResponse(id);

            RewardManager.Get().GiveReward(this.target, this.collect, this.amount, data.pos);
        }
    }
}