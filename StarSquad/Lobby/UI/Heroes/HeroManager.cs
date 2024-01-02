using System.Collections.Generic;
using StarSquad.Common.Hero;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using StarSquad.Net.Packet.Lobby.Hello;
using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Heroes
{
    public class HeroManager : MonoBehaviour
    {
        public TMP_Text unlockedHeaderText;
        private string originalText;

        private int heroCount;

        public Transform unlockedHeroes;
        public Transform lockedHeroes;

        public HeroListItem heroPrefab;

        private Dictionary<int, HeroListItem> items = new();

        private void Start()
        {
            this.originalText = this.unlockedHeaderText.text;

            if (!LoaderManager.IsUsingNet())
            {
                var heroes = new Dictionary<int, LobbyDataIncomingPacket.HeroInfo>();

                var gunman = new LobbyDataIncomingPacket.HeroInfo();
                gunman.id = 0;
                gunman.rating = 0;
                gunman.level = 1;
                heroes.Add(0, gunman);

                /*var smasher = new LobbyDataIncomingPacket.HeroInfo();
                smasher.id = 1;
                smasher.rating = 0;
                smasher.level = 1;
                heroes.Add(1, smasher);
                */

                this.AddHeroes(heroes);
            }
        }

        private void UpdateUnlockedAmount()
        {
            var count = 0;
            foreach (var item in this.items.Values)
            {
                if (!item) continue;
                if (!item.IsLocked()) ++count;
            }

            this.unlockedHeaderText.text = string.Format(this.originalText, count, this.heroCount);
        }

        public void OnHeroUnlocked(int id, int rating, int level)
        {
            var hero = this.items[id];
            if (hero == null) return;
            hero.SetUnlocked(rating, level);

            hero.transform.SetParent(this.unlockedHeroes);

            /*int lastSiblingIndex = 0;
            foreach (HeroListItem child in this.unlockedHeroes)
            {
                if (id > child.id)
                {
                    lastSiblingIndex = child.id;
                    continue;
                }

                if (id == child.id)
                {
                    hero.transform.SetSiblingIndex(lastSiblingIndex + 1);
                }
                else
                {
                    child.transform.SetSiblingIndex(child.transform.GetSiblingIndex() + 1);
                }
            }*/

            this.UpdateUnlockedAmount();
        }

        public void OnUpgrade(int id, int level)
        {
            var hero = this.items[id];
            if (hero == null) return;
            hero.OnUpgrade(level);
        }

        public void AddHeroes(Dictionary<int, LobbyDataIncomingPacket.HeroInfo> heroes)
        {
            var staticHeroData = StaticHeroDataManager.Get().heroes;
            foreach (var staticHero in staticHeroData.Values)
            {
                var unlocked = heroes.TryGetValue(staticHero.id, out var val);
                var hero = Instantiate(this.heroPrefab, unlocked ? this.unlockedHeroes : this.lockedHeroes);

                var bundle = AssetManager.LoadAssetBundle0(staticHero.bundle);
                var sprite = bundle.LoadAsset<Sprite>(staticHero.card);

                hero.image.sprite = sprite;

                //AssetManager.UnloadAssetBundle0(staticHero.bundle, false);

                hero.Init(staticHero.id);

                if (unlocked)
                {
                    hero.SetUnlocked(val.rating, val.level);
                }
                else
                {
                    hero.SetLocked();
                }

                this.items.Add(staticHero.id, hero);

                ++this.heroCount;
            }

            this.UpdateUnlockedAmount();
        }

        public HeroListItem GetItem(int id)
        {
            return this.items[id];
        }
        
        public static HeroManager Get()
        {
            return LobbyManager.instance.heroManager;
        }
    }
}