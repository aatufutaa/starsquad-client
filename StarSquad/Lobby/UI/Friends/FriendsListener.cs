using System;
using System.Collections.Generic;
using StarSquad.Loader;
using StarSquad.Lobby.Confirm.Friend;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Lobby.UI.Tooltip;
using StarSquad.Lobby.UI.Util;
using StarSquad.Net;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Friends
{
    public class FriendsListener : MonoBehaviour
    {
        private readonly Dictionary<string, Friend> friends = new Dictionary<string, Friend>();
        public FriendListItem prefab;

        // header
        public CustomButton friendsButton;
        public CustomButton invitesButton;

        public TMP_Text friendsButtonText;
        public TMP_Text invitesButtonText;
        private Color activeButtonColor;
        private Color inactiveButtonColor;

        public Transform selectedBar;

        private bool friendsPageSelected = true;
        private bool animating;
        private float timer;
        private float fromX;

        // content
        public RectTransform friendsPage;
        public RectTransform invitesPage;

        public ScrollRect scrollRect;

        // friends
        public TooltipBase selected;
        private string selectedPlayerId;

        public TMP_Text noFriendsText;

        // invites
        public IncomingFriendInviteItem incomingPrefab;
        public OutgoingFriendInviteItem outgoingPrefab;

        public Transform incomingHeader;
        public TMP_Text incomingText;
        private string incomingFormat;
        public Transform outgoingHeader;
        public TMP_Text outgoingText;
        private string outgoingFormat;

        private readonly Dictionary<string, IncomingFriendInviteItem> incomingFriendInvites = new();
        private readonly Dictionary<string, OutgoingFriendInviteItem> outgoingFriendInvites = new();

        // add friend
        public TMP_Text playerIdText;
        public TMP_InputField input;
        public ActiveButton inviteButton;

        private void Awake()
        {
            string playerId;
            if (LoaderManager.IsUsingNet())
                playerId = NetworkManager.GetNet().sessionManager.playerId;
            else
                playerId = "123";
            this.playerIdText.text = string.Format(this.playerIdText.text, playerId);

            this.friendsButton.onClick.AddListener(this.HandleFriends);
            this.invitesButton.onClick.AddListener(this.HandleInvites);

            this.activeButtonColor = this.friendsButtonText.color;
            this.inactiveButtonColor = this.invitesButtonText.color;

            this.incomingFormat = this.incomingText.text;
            this.outgoingFormat = this.outgoingText.text;

            this.inviteButton.onClick.AddListener(this.HandleAdd);

            this.selected.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!this.animating) return;

            this.timer += Time.deltaTime;

            const float animationTime = 0.3f;
            var p = this.timer / animationTime;

            if (p >= 1f)
            {
                p = 1f;
                this.animating = false;
            }

            p = MathHelper.SmoothLerp(p);

            var fromX = this.fromX;

            var targetX = this.friendsPageSelected
                ? this.friendsButton.transform.localPosition.x
                : this.invitesButton.transform.localPosition.x;

            var current = fromX + (targetX - fromX) * p;

            this.selectedBar.localPosition = new Vector3(current, this.selectedBar.localPosition.y);
        }

        private class Friend
        {
            public int rating;
            public FriendListItem item;
        }

        public void AddFriend(string playerId, string name, int rating,
            UpdateFriendStatusIncomingPacket.FriendStatus status)
        {
            if (this.friends.ContainsKey(playerId))
            {
                this.RemoveFriend(playerId);
            }

            var friend = new Friend();
            friend.rating = rating;

            var item = Instantiate(this.prefab, this.friendsPage.transform);
            friend.item = item;
            item.SetData(playerId, name, rating);
            item.SetStatus(status);

            this.friends.Add(playerId, friend);

            this.noFriendsText.gameObject.SetActive(false);

            Debug.Log("added friend " + playerId);
        }

        public void RemoveFriend(string playerId)
        {
            if (!this.friends.TryGetValue(playerId, out var friend)) return;
            Destroy(friend.item.gameObject);
            this.friends.Remove(playerId);

            if (this.friends.Count == 0)
                this.noFriendsText.gameObject.SetActive(true);
        }

        public void UpdateStatus(string playerId, UpdateFriendStatusIncomingPacket.FriendStatus status)
        {
            if (!this.friends.ContainsKey(playerId))
            {
                Debug.Log("cant update friend status for " + playerId);
                return;
            }

            this.friends[playerId].item.SetStatus(status);
        }

        /*public void UpdateRating(string playerId, int rating)
        {
            this.friends[playerId].item.SetRating(rating);
        }*/

        private void StartSelectedAnimation()
        {
            this.fromX = this.selectedBar.localPosition.y;
            this.animating = true;
            this.timer = 0f;
        }

        // invites
        public void AddIncomingInvite(AddFriendIncomingPacket.Friend friend, bool updateText)
        {
            if (this.incomingFriendInvites.ContainsKey(friend.playerId))
            {
                this.RemoveIncomingInvite(friend.playerId);
            }

            var item = Instantiate(this.incomingPrefab, this.invitesPage);
            item.SetData(friend.playerId, friend.name, friend.rating);
            item.transform.SetSiblingIndex(this.incomingHeader.GetSiblingIndex() + 1);

            this.incomingFriendInvites.Add(friend.playerId, item);

            if (updateText)
                this.UpdateIncomingText();
        }

        public void UpdateIncomingText()
        {
            this.incomingText.text = string.Format(this.incomingFormat, this.incomingFriendInvites.Count);
        }

        public void AddOutgoingInvite(AddFriendIncomingPacket.Friend friend, bool updateText)
        {
            if (this.outgoingFriendInvites.ContainsKey(friend.playerId))
            {
                this.RemoveOutgoingInvite(friend.playerId);
            }

            var item = Instantiate(this.outgoingPrefab, this.invitesPage);
            item.SetData(friend.playerId, friend.name, friend.rating);
            item.transform.SetSiblingIndex(this.outgoingHeader.GetSiblingIndex() + 1);

            this.outgoingFriendInvites.Add(friend.playerId, item);

            if (updateText)
                this.UpdateOutgoingText();
        }

        public void UpdateOutgoingText()
        {
            this.outgoingText.text = string.Format(this.outgoingFormat, this.outgoingFriendInvites.Count);
        }

        public void RemoveIncomingInvite(string playerId)
        {
            if (!this.incomingFriendInvites.TryGetValue(playerId, out var item)) return;
            this.incomingFriendInvites.Remove(playerId);
            Destroy(item.gameObject);
            this.UpdateIncomingText();
        }

        public void RemoveOutgoingInvite(string playerId)
        {
            if (!this.outgoingFriendInvites.TryGetValue(playerId, out var item)) return;
            this.outgoingFriendInvites.Remove(playerId);
            Destroy(item.gameObject);
            this.UpdateOutgoingText();
        }

        // buttons
        private void HandleFriends()
        {
            if (this.friendsPageSelected) return;

            this.friendsPage.gameObject.SetActive(true);
            this.invitesPage.gameObject.SetActive(false);

            this.scrollRect.content = this.friendsPage;
            this.scrollRect.normalizedPosition = Vector2.zero;

            this.friendsButtonText.color = this.activeButtonColor;
            this.invitesButtonText.color = this.inactiveButtonColor;

            this.friendsPageSelected = true;
            this.StartSelectedAnimation();
        }

        private void HandleInvites()
        {
            if (!this.friendsPageSelected) return;

            this.friendsPage.gameObject.SetActive(false);
            this.invitesPage.gameObject.SetActive(true);

            this.scrollRect.content = this.invitesPage;
            this.scrollRect.normalizedPosition = Vector2.zero;

            this.friendsButtonText.color = this.inactiveButtonColor;
            this.invitesButtonText.color = this.activeButtonColor;

            this.friendsPageSelected = false;
            this.StartSelectedAnimation();
        }

        private void HandleAdd()
        {
            var playerId = this.input.text;

            //this.inviteButton.SetDisabled();

            if (LoaderManager.IsUsingNet())
                PacketConfirmManager.Get().Send(new InviteFriendOutgoingPacket(playerId));
            else
                this.AddFriend("test123", "name123", 100, 0);
        }

        public void ShowSelected(Transform transform, string playerId)
        {
            TooltipManager.Get().Show(this.selected);
            this.selectedPlayerId = playerId;
            this.selected.transform.position = new Vector3(this.selected.transform.position.x, transform.position.y);
        }

        // these get called from selected
        public void ShowSelectedProfile()
        {
            ScreenManager.GetScreenManager().ShowProfile(this.selectedPlayerId);
        }

        public void RemoveSelected()
        {
            PacketConfirmManager.Get().Send(new RemoveFriendOutgoingPacket(this.selectedPlayerId));
        }

        public void SpectateSelected()
        {
        }
    }
}