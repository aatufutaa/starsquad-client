using StarSquad.Loader;
using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Friend
{
    public class FriendResponseIncomingPacket : IncomingPacket
    {
        private enum FriendResponse
        {
            InvalidPlayerId,
            
            AlreadyFriends,
            AlreadyInvited,
            OtherAlreadyInvited,
            CantInviteSelf,
            TooManyInvites,
            OtherTooManyInvites,
            TooManyFriends,
            OtherTooManyFriends,
            FailedToInvite,
            NotAllowingFriend,
            
            NotInvited,
            FailedToAccept,
            
            NotFriend,
            FailedToRemove,
        }

        private FriendResponse response;

        public void Read(ByteBuf buf)
        {
            this.response = (FriendResponse)buf.ReadByte();
        }

        public void Handle()
        {
            string msg;

            switch (this.response)
            {
                case FriendResponse.InvalidPlayerId:
                    msg = "Please insert a valid player ID";
                    break;
                
                case FriendResponse.AlreadyFriends:
                    msg = "You are already friends with this player";
                    break;
                
                case FriendResponse.AlreadyInvited:
                    msg = "You have already sent a friend invite to this player";
                    break;
                
                case FriendResponse.OtherAlreadyInvited:
                    msg = "This player has already sent you an invite";
                    break;
                
                case FriendResponse.CantInviteSelf:
                    msg = "You can't invite yourself";
                    break;
                
                case FriendResponse.TooManyInvites:
                    msg = "You have sent too many friend invites";
                    break;
                
                case FriendResponse.OtherTooManyInvites:
                    msg = "This player has too many friend invites";
                    break;
                
                case FriendResponse.TooManyFriends:
                    msg = "Your friend list is full";
                    break;
                
                case FriendResponse.OtherTooManyFriends:
                    msg = "This player's friend list is full";
                    break;
                
                case FriendResponse.FailedToInvite:
                    msg = "Failed to invite this player";
                    break;
                
                case FriendResponse.NotAllowingFriend:
                    msg = "This player is not allowing friend requests";
                    break;
                
                case FriendResponse.NotInvited:
                    msg = "This player has not invited you";
                    break;
                
                case FriendResponse.FailedToAccept:
                    msg = "Failed to accept this invite";
                    break;
                
                case FriendResponse.NotFriend:
                    msg = "This player is not your friend";
                    break;
                
                case FriendResponse.FailedToRemove:
                    msg = "Failed to remove friend";
                    break;
                
                default:
                    msg = "Dont know what to say with response " + this.response;
                    break;
            }
            
            LoaderManager.instance.alertManager.Alert(msg);
        }
    }
}