using System.Collections.Generic;
using StarSquad.Game.Misc;
using StarSquad.Loader.Asset;
using TMPro;
using UnityEngine;

namespace StarSquad.Game.Mode
{
    public class TutorialGame : GameMode
    {
        private readonly GameObject spinner;
        private readonly Vector3 offset;

        private readonly GameObject bubble;
        private readonly TMP_Text text;

        private int stage;

        private Dictionary<int, string> msg;

        private CustomAssetBundle tutorialBundle;

        public TutorialGame()
        {
            this.tutorialBundle = AssetManager.LoadAssetBundle0("tutorial_bundle");

            this.spinner = Object.Instantiate(this.LoadTutorialAsset("Spinner.prefab"));
            this.spinner.SetActive(false);
            AngleUtil.FixAngle(this.spinner.transform, 5);
            this.offset = this.spinner.transform.position;

            var canvas = GameObject.Find("CanvasGame");
            this.bubble = Object.Instantiate(this.LoadTutorialAsset("Bubble.prefab"), canvas.transform);
            this.text = this.bubble.transform.Find("Text").GetComponent<TMP_Text>();

            this.msg = new Dictionary<int, string>();
        }

        private GameObject LoadTutorialAsset(string path)
        {
            return this.tutorialBundle.LoadAsset<GameObject>("Assets/Remote/Game/Tutorial/Access/" + path);
        }

        public void UpdateSpinner(float x, float y)
        {
            this.spinner.transform.position = new Vector3(x, 0, y) + this.offset;
            this.spinner.SetActive(true);
        }

        public void HideSpinner()
        {
            this.spinner.SetActive(false);
        }

        public void SetStage(int stage)
        {
            if (this.stage != stage)
            {
                // TODO: play next stage sound
                // TODO: only play for some stage
            }

            this.stage = stage;

            string msg;
            if (!this.msg.TryGetValue(stage, out msg))
                msg = "dont know what to do with stage " + stage;

            this.text.text = msg;
        }

        public override void Tick()
        {
        }

        public override void Render()
        {
        }

        public override DynamicGameData GetDynamicGameData()
        {
            return new TutorialDynamicGameData();
        }
    }
}