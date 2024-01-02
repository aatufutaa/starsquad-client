using StarSquad.Loader;
using StarSquad.Lobby.Party;
using StarSquad.Lobby.Party.Packet;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Screens.Party;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Screens
{
    public class PartyScreen : GameScreen
    {
        // no part
        public GameObject notInParty;
        public ActiveButton createButton;
        private bool created;
        public TMP_InputField input;
        public ActiveButton joinButton;

        // yes part
        public GameObject inParty;
        public ActiveButton leaveButton;
        public TMP_Text partyCodeText;
        // members
        public Transform membersParent;
        public PartyMemberListObject memberPrefab;

        public void Init()
        {
            this.createButton.onClick.AddListener(this.HandleCreate);
            this.leaveButton.onClick.AddListener(this.HandleLeave);
            this.joinButton.onClick.AddListener(this.HandleJoin);
        }

        // when create button pressed
        private void HandleCreate()
        {
            if (this.created) return;

            this.created = true;

            this.createButton.SetDisabled();

            if (LoaderManager.IsUsingNet())
            {
                PacketConfirmManager.Get().Send(new CreatePartyOutgoingPacket());
            }
            else
            {
                this.ResetCreateButton();
                this.OnPartyCreated("1234", true);
            }
        }

        // when leave button pressed
        private void HandleLeave()
        {
            this.leaveButton.SetDisabled();

            if (LoaderManager.IsUsingNet())
            {
                PacketConfirmManager.Get().Send(new LeavePartyOutgoingPacket());
            }
            else
            {
                this.ResetLeaveButton();
                this.OnPartyDestroyed();
            }
        }

        // when join button pressed
        private void HandleJoin()
        {
            var partyCode = this.input.text;

            if (partyCode.Length < 8 || partyCode.Length > 9)
            {
                LoaderManager.instance.alertManager.Alert("Please insert a valid party code");
                return;
            }

            this.joinButton.SetDisabled();

            if (LoaderManager.IsUsingNet())
            {
                PacketConfirmManager.Get().Send(new JoinPartyOutgoingPacket(partyCode));
            }
            else
            {
                this.ResetJoinButton();
                //this.OnPartyJoin("123");
            }
        }

        public void ResetCreateButton()
        {
            this.created = false;
            this.createButton.SetEnabled();
        }

        public void OnPartyCreated(string code, bool leader)
        {
            this.SetPartyCode(code);

            this.inParty.SetActive(true);
            this.notInParty.SetActive(false);

            var gameObject = Instantiate(this.memberPrefab, this.membersParent);
            var lobbyData = LobbyManager.instance.lobbyData;
            gameObject.SetData(lobbyData.name, lobbyData.totalTrophies);
            gameObject.SetRole(leader);
        }

        public void ResetLeaveButton()
        {
            this.leaveButton.SetEnabled();
        }

        public void OnPartyDestroyed()
        {
            this.inParty.SetActive(false);
            this.notInParty.SetActive(true);

            foreach (Transform child in this.membersParent)
            {
                Destroy(child.gameObject);
            }
        }

        public void ResetJoinButton()
        {
            this.joinButton.SetEnabled();
        }

        private void SetPartyCode(string partyCode)
        {
            this.partyCodeText.text = "#" + partyCode;
        }

        public void OnOtherPlayerJoin(PartyMember member, bool leader)
        {
            var gameObject = Instantiate(this.memberPrefab, this.membersParent);
            member.item = gameObject;

            member.item.SetData(member.name, member.trophies);
            member.item.SetRole(leader);

            // TODO: make sure this works and if doesnt fix
            // sort highest rating first
            foreach (PartyMemberListObject child in this.membersParent)
            {
                if (child.gameObject == gameObject.gameObject) continue;
                if (child.trophies < member.trophies)
                {
                    //var before = child.transform.GetSiblingIndex();
                    gameObject.transform.SetSiblingIndex(child.transform.GetSiblingIndex());

                    //Debug.Log(before + "and" + child.transform.GetSiblingIndex());
                    // child.transform.SetSiblingIndex(child.transform.GetSiblingIndex() + 1);
                }
            }
        }

        public void OnOtherPlayerLeave(PartyMember member)
        {
            Destroy(member.item.gameObject);
        }

        public void OnOtherPlayerUpdate(PartyMember member)
        {
            member.item.SetData(member.name, member.trophies);
        }

        public void CopyCodeToClipboard()
        {
            var partyCode = this.partyCodeText.text;

            var te = new TextEditor();
            te.text = partyCode;
            te.SelectAll();
            te.Copy();
        }
    }
}