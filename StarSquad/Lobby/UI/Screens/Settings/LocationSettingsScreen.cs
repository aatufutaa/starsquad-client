using System;
using System.Collections.Generic;
using StarSquad.Loader;
using UnityEngine;

namespace StarSquad.Lobby.UI.Screens.Settings
{
    public class LocationSettingsScreen : GameScreen
    {
        [NonSerialized] public int location;

        public LocationButton prefab;
        public Transform parent;

        private LocationButton[] locations;

        public void Init(List<string> locations)
        {
            this.locations = new LocationButton[locations.Count];

            for (var i = 0; i < locations.Count; i++)
            {
                var locationButton = Instantiate(this.prefab, this.parent);
                locationButton.text.text = locations[i];
                locationButton.SetEnabled(false);
                var id = i;
                locationButton.onClick.AddListener(() => this.HandleSelect(id));
                this.locations[i] = locationButton;
            }

#if UNITY_EDITOR
            if (!LoaderManager.IsUsingNet())
            {
                this.SetSelectedLocation(0);
            }
#endif
        }

        public void SetSelectedLocation(int location)
        {
            this.locations[this.location].SetEnabled(false);

            var locationButton = this.locations[location];
            locationButton.SetEnabled(true);

            this.location = location;
            
            ScreenManager.GetScreenManager().settings.locationButtonText.text = locationButton.text.text;
        }

        private void HandleSelect(int location)
        {
            this.SetSelectedLocation(location);
            ScreenManager.GetScreenManager().settings.SendSettings();
        }
    }
}