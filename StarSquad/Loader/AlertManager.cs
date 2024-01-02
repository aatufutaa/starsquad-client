using TMPro;
using UnityEngine;

namespace StarSquad.Loader
{
    public class AlertManager : MonoBehaviour
    {
        public GameObject prefab;

        private LoaderAlert[] alerts;
        private int alertIndex;

        private bool ticking;

        public void Init()
        {
            this.alerts = new LoaderAlert[10];
            for (var i = 0; i < this.alerts.Length; i++)
            {
                this.alerts[i] = new LoaderAlert(Instantiate(this.prefab, this.transform));
            }
            
            Destroy(this.prefab);
        }

        public class LoaderAlert
        {
            public GameObject gameObject;
            private TMP_Text text;
            private Vector2 startPos;
            
            private float time;
            public bool animating;

            public LoaderAlert(GameObject gameObject)
            {
                this.gameObject = gameObject;
                this.text = gameObject.GetComponent<TMP_Text>();
                this.startPos = gameObject.transform.localPosition;
            }

            public void Show(string msg)
            {
                this.text.text = msg;
                this.gameObject.SetActive(true);
                this.time = 0f;
                this.animating = true;
                this.gameObject.transform.localPosition = this.startPos;
                this.text.alpha = 1f;
            }

            public void Update()
            {
                if (!this.animating) return;

                this.time += Time.deltaTime;
                this.gameObject.transform.localPosition += new Vector3(0f, Time.deltaTime * 50f);

                const float fadeAfter = 2f;
                if (this.time < fadeAfter) return;

                var time = this.time - fadeAfter;

                const float fadeTime = 0.3f;
                var p = time / fadeTime;

                if (p > 1f)
                {
                    p = 1f;
                    this.animating = false;
                    this.gameObject.SetActive(false);
                }
                
                p = 1f - p;

                this.text.alpha = p;
            }
        }

        public void Alert(string msg)
        {
            if (this.alerts == null) return;

            Debug.Log("show alert " + msg);

            foreach (var other in this.alerts)
            {
                if (!other.animating) continue;
                other.gameObject.transform.localPosition += new Vector3(0f, 32f);
            }
            
            var alert = this.alerts[this.alertIndex++];
            if (this.alertIndex >= this.alerts.Length) this.alertIndex = 0;

            alert.Show(msg);

            this.ticking = true;
        }

        private void Update()
        {
            if (!this.ticking) return;

            var count = 0;
            
            foreach (var alert in this.alerts)
            {
                alert.Update();
                if (alert.animating) ++count;
            }

            this.ticking = count > 0;
        }

        public void AddTestAlert()
        {
            this.Alert("asd");
        }
    }
}