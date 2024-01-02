using System;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Screens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Heroes
{
    public class HeroListItem : CustomButton
    {
        public GameObject rating;
        public TMP_Text levelText;
        public TMP_Text ratingText;
        public Image image;

        private Material mat;

        [NonSerialized] public int id;
        private int ratingAmount;
        private int level = 1;
        
        public void Init(int id)
        {
            this.id = id;
            
            this.mat = this.image.material;

            this.onClick.AddListener(this.HandleHero);
        }

        public void SetLocked()
        {
            this.rating.SetActive(false);
            this.image.material = LobbyManager.instance.grayscaleMaterial;
        }

        public void SetUnlocked(int rating, int level)
        {
            this.rating.SetActive(true);
            this.ratingText.text = "" + rating;
            this.image.material = this.mat;

            this.ratingAmount = rating;

            this.OnUpgrade(level);
        }

        public void OnUpgrade(int level)
        {
            this.level = level;
            this.levelText.text = "" + level;
        }

        private void HandleHero()
        {
            var hero = ScreenManager.GetScreenManager().hero;
            hero.SetValue(this.id, !this.IsLocked(), this.ratingAmount, this.level);
            hero.Show();
        }

        public bool IsLocked()
        {
            return !this.rating.activeSelf;
        }

        public int GetTrophies()
        {
            return this.ratingAmount;
        }

        public int GetLevel()
        {
            return this.level;
        }
    }
}