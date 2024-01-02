using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Button
{
    public class ActiveButton : CustomButton
    {
        private Image image;
        private Material originalMaterial;

        private bool init;

        private void Awake()
        {
            this.Init();
        }

        public void Init()
        {
            if (this.init) return;
            this.init = true;
            this.image = this.GetComponent<Image>();
            this.originalMaterial = this.image.material;
        }

        public void SetEnabled()
        {
            this.image.material = this.originalMaterial;
        }

        public void SetDisabled()
        {
            this.image.material = LobbyManager.instance.grayscaleMaterial;
        }
    }
}