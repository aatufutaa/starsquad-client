using StarSquad.Loader;
using StarSquad.Lobby.UI;
using StarSquad.Lobby.UI.Screens;

namespace StarSquad.Lobby.Party
{
    public class PartyManager
    {
        private PlayerParty party;

        public void CreateParty(string code, bool silent, bool leader)
        {
            this.party = new PlayerParty();

            ScreenManager.GetScreenManager().party.OnPartyCreated(code, leader);

            if (!silent)
                LoaderManager.instance.alertManager.Alert("You have created a party");
        }

        public void LeaveParty(bool leave)
        {
            this.party = null;

            ScreenManager.GetScreenManager().party.OnPartyDestroyed();

            LoaderManager.instance.alertManager.Alert(leave ? "You have left the party" : "The party was disbanded");
        }

        public void JoinParty(string code)
        {
            this.CreateParty(code, true, false);

            LoaderManager.instance.alertManager.Alert("You have joined the party");
        }

        public void OtherPlayerJoinParty(PartyMember member, bool silent, bool leader)
        {
            this.party.Join(member);

            ScreenManager.GetScreenManager().party.OnOtherPlayerJoin(member, leader);

            if (!silent)
                LoaderManager.instance.alertManager.Alert(member.name + " joined the party");
        }

        public void OtherPlayerLeaveParty(string playerId)
        {
            var member = this.party.Leave(playerId);
            if (member == null) return;
            
            ScreenManager.GetScreenManager().party.OnOtherPlayerLeave(member);
            
            LoaderManager.instance.alertManager.Alert(member.name + " left the party");
        }

        public void OtherPlayerUpdateParty(PartyMember member)
        {
            this.party.Update(member);
            
            ScreenManager.GetScreenManager().party.OnOtherPlayerUpdate(member);
        }
    }
}