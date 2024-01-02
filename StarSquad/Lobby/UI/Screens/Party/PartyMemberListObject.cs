using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Screens.Party
{
    public class PartyMemberListObject : MonoBehaviour
    {
        public TMP_Text usernameText;
        public TMP_Text roleText;
        public TMP_Text trophiesText;

        public int trophies;
        
        public void SetData(string name, int trophies)
        {
            this.usernameText.text = name;
            this.trophiesText.text = "" + trophies;

            this.trophies = trophies;
        }
        
        public void SetRole(bool leader)
        {
            this.roleText.text = leader ? "Leader" : "Member";
        }
    }
}