using TMPro;
using UnityEngine;

namespace StarSquad.Loader
{
    public class DebugManager : MonoBehaviour
    {
        public TMP_Text fpsText;
        public TMP_Text tpsText;
        
        private int fpsCounter;
        private float lastFpsReset;
        private int tpsCounter;
        private float lastTpsReset;

        private void Update()
        {
            ++this.fpsCounter;
            if (Time.time - this.lastFpsReset >= 1f)
            {
                this.lastFpsReset = Time.time;
                this.fpsText.text = "Fps: " + this.fpsCounter;
                this.fpsCounter = 0;
            }
        }

        private void FixedUpdate()
        {
            ++this.tpsCounter;
            if (Time.time - this.lastTpsReset >= 1f)
            {
                this.lastTpsReset = Time.time;
                this.tpsText.text = "Tps: " + this.tpsCounter;
                this.tpsCounter = 0;
            }
        }

        public void Reload()
        {
            LoaderManager.instance.Reload();
        }

        public void Reconnect()
        {
            LoaderManager.instance.networkManager.connectionManager.Disconnect("t");
        }
    }
}