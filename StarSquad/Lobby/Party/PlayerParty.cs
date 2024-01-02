using System.Collections.Generic;
using StarSquad.Loader;
using StarSquad.Lobby.UI.Screens.Party;
using StarSquad.Net.Packet;
using UnityEngine;

namespace StarSquad.Lobby.Party
{
    public class PlayerParty
    {
        private readonly Dictionary<string, PartyMember> members = new Dictionary<string, PartyMember>();

        public void Join(PartyMember member)
        {
            this.members.Add(member.playerId, member);
        }

        public void Update(PartyMember member)
        {
            var old = this.members[member.playerId];
            member.item = old.item;
            this.members[member.playerId] = member;
            
            LoaderManager.instance.alertManager.Alert(member.name + " updated the party");
        }
        
        public PartyMember Leave(string playerId)
        {
            var member = this.members[playerId];
            if (member == null) return null;

            this.members.Remove(playerId);

            return member;
        }

        public void Reset()
        {
            this.members.Clear();
        }
    }

    public class PartyMember
    {
        public string playerId;
        public string name;

        public int trophies;

        public int hero;
        public int heroTrophies;

        public PartyMemberListObject item;

        public void Read(ByteBuf buf)
        {
            this.playerId = buf.ReadString();
            this.name = buf.ReadString();

            this.trophies = buf.ReadInt();

            this.hero = buf.ReadByte();
            this.heroTrophies = buf.ReadShort();
        }
    }
}