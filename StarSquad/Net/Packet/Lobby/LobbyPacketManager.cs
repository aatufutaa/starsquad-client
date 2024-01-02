using StarSquad.Lobby.Confirm;
using StarSquad.Lobby.Confirm.Friend;
using StarSquad.Lobby.Confirm.Hero;
using StarSquad.Lobby.Confirm.HeroPass;
using StarSquad.Lobby.Confirm.Misc;
using StarSquad.Lobby.Confirm.Packet;
using StarSquad.Lobby.Confirm.Quests;
using StarSquad.Lobby.Confirm.Shop;
using StarSquad.Lobby.Party.Packet;
using StarSquad.Net.Confirm;
using StarSquad.Net.Packet.Lobby.Hello;
using StarSquad.Net.Packet.Lobby.Misc;
using StarSquad.Net.Packet.Lobby.Queue;
using StarSquad.Net.Packet.Play;
using UnityEngine;

namespace StarSquad.Net.Packet.Lobby
{
    public class LobbyPacketManager : ConfirmPacketManager
    {
        public LobbyPacketManager()
        {
            this.RegisterPackets();

            this.RegisterConfirmPackets();
        }

        private void RegisterPackets()
        {
            this.RegisterIncoming(0, typeof(HelloIncomingPacket)); // play packet
            this.RegisterOutgoing(0, typeof(HelloOutgoingPacket)); // play packet

            this.RegisterOutgoing(1, typeof(RequestDataOutgoingPacket));

            this.RegisterIncoming(2, typeof(SendToServerIncomingPacket)); // play packet
            this.RegisterIncoming(3, typeof(KickIncomingPacket)); // play packet
            this.RegisterIncoming(4, typeof(LobbyDataIncomingPacket));

            this.RegisterOutgoing(5, typeof(DataLoadedOutgoingPacket)); // play packet

            this.RegisterIncoming(12, typeof(QueueStatusIncomingPacket));

            this.RegisterIncoming(14, typeof(AlertIncomingPacket));
        }

        private void RegisterConfirmPackets()
        {
            // queue
            this.RegisterConfirmOutgoing(0, typeof(JoinQueueOutgoingPacket));
            this.RegisterConfirmIncoming(1, typeof(LeaveQueueIncomingPacket));
            this.RegisterConfirmOutgoing(1, typeof(LeaveQueueOutgoingPacket));

            // party
            this.RegisterConfirmIncoming(2, typeof(CreatePartyIncomingPacket));
            this.RegisterConfirmOutgoing(2, typeof(CreatePartyOutgoingPacket));
            this.RegisterConfirmIncoming(3, typeof(JoinPartyIncomingPacket));
            this.RegisterConfirmOutgoing(3, typeof(JoinPartyOutgoingPacket));
            this.RegisterConfirmIncoming(4, typeof(LeavePartyIncomingPacket));
            this.RegisterConfirmOutgoing(4, typeof(LeavePartyOutgoingPacket));
            this.RegisterConfirmIncoming(5, typeof(PlayerJoinPartyIncomingPacket));
            this.RegisterConfirmIncoming(6, typeof(PlayerLeavePartyIncomingPacket));
            //RegisterIncoming(7, typeof(PlayerUpdatePartyIncomingPacket));

            this.RegisterConfirmIncoming(7, typeof(SetNameIncomingPacket));
            this.RegisterConfirmOutgoing(7, typeof(SetNameOutgoingPacket));

            // friends
            this.RegisterConfirmOutgoing(8, typeof(InviteFriendOutgoingPacket));

            this.RegisterConfirmIncoming(8, typeof(AddFriendInviteIncomingPacket));
            this.RegisterConfirmIncoming(9, typeof(RemoveFriendInviteIncomingPacket));

            this.RegisterConfirmIncoming(10, typeof(AddFriendIncomingPacket));
            this.RegisterConfirmIncoming(11, typeof(RemoveFriendIncomingPacket));

            this.RegisterConfirmOutgoing(12, typeof(AcceptInviteOutgoingPacket));

            this.RegisterConfirmIncoming(13, typeof(FriendResponseIncomingPacket));

            this.RegisterConfirmIncoming(14, typeof(UpdateFriendStatusIncomingPacket));

            this.RegisterConfirmOutgoing(15, typeof(CancelFriendInviteOutgoingPacket));

            this.RegisterConfirmOutgoing(16, typeof(RemoveFriendOutgoingPacket));

            // other
            this.RegisterConfirmIncoming(17, typeof(RequestProfileIncomingPacket));
            this.RegisterConfirmOutgoing(17, typeof(RequestProfileOutgoingPacket));

            this.RegisterConfirmIncoming(18, typeof(ClaimLevelRewardIncomingPacket));
            this.RegisterConfirmOutgoing(18, typeof(ClaimLevelRewardOutgoingPacket));

            this.RegisterConfirmIncoming(19, typeof(CancelRewardIncomingPacket));
            this.RegisterConfirmIncoming(20, typeof(RewardIncomingPacket));

            this.RegisterConfirmIncoming(21, typeof(UpgradeHeroIncomingPacket));
            this.RegisterConfirmOutgoing(21, typeof(UpgradeHeroOutgoingPacket));
            this.RegisterConfirmIncoming(22, typeof(CancelUpgradeHeroIncomingPacket));

            this.RegisterConfirmIncoming(23, typeof(RemoveCoinsIncomingPacket));

            this.RegisterConfirmOutgoing(24, typeof(UpdateSettingsOutgoingPacket));

            this.RegisterConfirmIncoming(25, typeof(QuestIncomingPacket));
            this.RegisterConfirmOutgoing(25, typeof(QuestOutgoingPacket));
            this.RegisterConfirmIncoming(26, typeof(CancelQuestIncomingPacket));

            this.RegisterConfirmIncoming(27, typeof(CancelPurchaseIncomingPacket));
            this.RegisterConfirmOutgoing(27, typeof(BuyShopItemOutgoingPacket));

            this.RegisterConfirmIncoming(28, typeof(ClaimProgressionIncomingPacket));
            this.RegisterConfirmOutgoing(28, typeof(ClaimProgressionOutgoingPacket));
            this.RegisterConfirmIncoming(29, typeof(CancelClaimProgressionIncomingPacket));

            this.RegisterConfirmIncoming(30, typeof(ClaimHeroPassRewardIncomingPacket));
            this.RegisterConfirmOutgoing(30, typeof(ClaimHeroPassRewardOutgoingPacket));
            this.RegisterConfirmIncoming(31, typeof(CancelHeroPassRewardIncomingPacket));

            this.RegisterConfirmIncoming(32, typeof(BuyNextTierHeroPassIncomingPacket));
            this.RegisterConfirmOutgoing(32, typeof(BuyNextTierHeroPassOutgoingPacket));
            this.RegisterConfirmIncoming(33, typeof(CancelBuyNextTierHeroPassIncomingPacket));

            this.RegisterConfirmIncoming(34, typeof(BuyHeroPassIncomingPacket));
            this.RegisterConfirmOutgoing(34, typeof(BuyHeroPassOutgoingPacket));
            this.RegisterConfirmIncoming(35, typeof(CancelBuyHeroPassIncomingPacket));
            
            this.RegisterConfirmIncoming(40, typeof(LinkGameCenterIncomingPacket));
            this.RegisterConfirmOutgoing(40, typeof(LinkGameCenterOutgoingPacket));
        }
    }
}