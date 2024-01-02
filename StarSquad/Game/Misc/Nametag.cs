using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Game.Misc
{
    public class Nametag
    {
        public enum NametagColor
        {
            Green,
            Blue,
            Red
        }

        protected Transform transform;
        private Vector3 offset;

        private float height;

        public HealthBar healthBar;
        public ReloadBar reloadBar;

        public Nametag(GameWorld world, NametagColor color, float height, string name)
        {
            var canvas = GameObject.Find("NametagCanvas");
            var colorName = color == NametagColor.Green ? "green" : color == NametagColor.Blue ? "base" : "red";
            this.transform = Object
                .Instantiate(world.LoadGeneralGameAsset("NameTag/" + colorName + ".prefab"), canvas.transform)
                .transform;
            AngleUtil.FixAngle(this.transform, -5);
            this.offset = this.transform.position;

            this.height = height;

            this.transform.Find("Name").GetComponent<TMP_Text>().text = name;

            this.healthBar = new HealthBar(this.transform);

            if (color == NametagColor.Green)
            {
                this.reloadBar = new ReloadBar(this.transform);
            }
        }

        public void SetPosition(float x, float y)
        {
            this.transform.position = new Vector3(x, this.height, y) + this.offset;
        }

        public void Tick()
        {
            this.healthBar.Tick();
        }

        public void Render()
        {
            this.healthBar.Render();
            this.reloadBar?.Render();
        }

        public void Reset()
        {
            this.healthBar.Reset();
        }

        public void Show(bool show)
        {
            this.transform.gameObject.SetActive(show);
        }
    }

    public class HealthBar
    {
        private Material fill;
        private Material preFill;
        private GameObject preFillGameObject;
        private TMP_Text amount;

        private int lastDamageTick;
        private float fromPreFill;
        private float toPreFill;
        private float damageTimer;
        private bool animatePreFill;

        public HealthBar(Transform parent)
        {
            var healthBar = parent.Find("Healthbar");
            this.fill = MaterialUtil.DuplicateMaterial(healthBar.Find("Fill").GetComponent<Image>());
            this.preFillGameObject = healthBar.Find("PreFill").gameObject;
            this.preFill = MaterialUtil.DuplicateMaterial(this.preFillGameObject.GetComponent<Image>());
            this.amount = healthBar.Find("Amount").GetComponent<TMP_Text>();
        }

        public void Render()
        {
            if (!this.animatePreFill || this.lastDamageTick < 10) return;

            this.damageTimer += Time.deltaTime;

            const float preFillTime = 0.3f;
            var p = this.damageTimer / preFillTime;
            var current = this.fromPreFill + (this.toPreFill - this.fromPreFill) * p;

            this.SetPreProgress(current);

            if (p >= 1f)
            {
                this.animatePreFill = false;
                this.preFillGameObject.SetActive(false);
            }
        }

        public void Tick()
        {
            ++this.lastDamageTick;
        }

        public void Reset()
        {
        }

        private void SetProgress(float p)
        {
            this.fill.SetFloat("_Progress", p);
        }

        private void SetPreProgress(float p)
        {
            this.preFill.SetFloat("_Progress", p);
        }

        private float GetProgress()
        {
            return this.fill.GetFloat("_Progress");
        }

        private float GetPreProgress()
        {
            return this.preFill.GetFloat("_Progress");
        }

        public void SetHealthAndReset(int health, int maxHealth)
        {
            this.SetHealth(health, maxHealth);
            this.SetPreProgress(this.GetProgress());
        }

        private void SetHealth(int health, int maxHealth)
        {
            var p = health / (float)maxHealth;
            this.SetProgress(p);

            var displayHealth = Mathf.Ceil(maxHealth * p);
            this.amount.text = "" + displayHealth;
        }

        public void Damage(int health, int maxHealth)
        {
            this.SetHealth(health, maxHealth);

            this.lastDamageTick = 0;
            this.damageTimer = 0f;
            this.animatePreFill = true;
            this.fromPreFill = this.GetPreProgress();
            this.toPreFill = this.GetProgress();
            this.preFillGameObject.SetActive(true);
        }
    }

    public class ReloadBar
    {
        private GameObject reloadbar;
        private Image image;

        private Progress[] progress;

        private float noReloadTime;
        private int repeat;

        public class Progress
        {
            private Image fill;
            private Material mat;

            public Progress(Transform parent)
            {
                this.fill = parent.Find("Fill").GetComponent<Image>();
                this.mat = MaterialUtil.DuplicateMaterial(this.fill);
            }

            public void SetValue(float p)
            {
                this.mat.SetFloat("_Progress", p);
                this.fill.color = new Color(1f, 1f, 1f, p < 1f ? 0.5f : 1f);
            }
        }

        public ReloadBar(Transform parent)
        {
            this.reloadbar = parent.Find("Reloadbar").gameObject;
            this.image = this.reloadbar.GetComponent<Image>();

            this.progress = new Progress[4];
            for (var i = 0; i < this.progress.Length; i++)
            {
                var progress = new Progress(this.reloadbar.transform.GetChild(i));
                this.progress[i] = progress;
            }
        }

        public void Render()
        {
            if (this.repeat <= 0) return;

            this.noReloadTime += Time.deltaTime;

            const float timer = 0.2f;
            var p = this.noReloadTime / timer;
            if (p > 1f)
            {
                p = 1f;
                --this.repeat;
                this.noReloadTime = 0f;
            }

            if (p > 0.5f)
            {
                p -= 0.5f;
                p /= 0.5f;
                p = 1f - p;
            }
            else
            {
                p /= 0.5f;
            }

            const int maxX = -20;
            this.reloadbar.transform.localPosition =
                new Vector2(maxX * p, this.reloadbar.transform.localPosition.y);

            this.image.color = new Color(1f, 1f - p, 1f - p);
        }

        public void SetProgress(int reloads, float p)
        {
            for (var i = 0; i < this.progress.Length; i++)
            {
                var progress = this.progress[i];

                if (i <= reloads)
                {
                    progress.SetValue(1f);
                }
                else
                {
                    progress.SetValue(0f);
                }
            }

            if (reloads < this.progress.Length)
            {
                this.progress[reloads].SetValue(p);
            }
        }

        public void PlayNoReloadAnimation()
        {
            // TODO: play sou

            this.repeat = 2;
        }
    }
}