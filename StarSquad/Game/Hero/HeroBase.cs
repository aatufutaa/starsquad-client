using System.Collections.Generic;
using StarSquad.Common.Hero;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using StarSquad.Lobby.Hero;
using UnityEditor;
using UnityEngine;

namespace StarSquad.Game.Hero
{
    public abstract class HeroBase
    {
        public float attackHeight;
        public float nametagHeight;

        protected Animator animator;

        private readonly List<RendererInfo> renderers = new();

        public StaticHeroDataManager.StaticHero staticHero;
        public CustomAssetBundle assetBundle;

        public int lastAttack;

        public int id;
        
        public class RendererInfo
        {
            public Renderer renderer;
            public Material[] originalMaterials;
            public Material[] transparentMaterials;

            public RendererInfo(Material transparentMaterial, Renderer renderer)
            {
                this.renderer = renderer;
                this.originalMaterials = this.renderer.sharedMaterials;
                this.transparentMaterials = new Material[this.originalMaterials.Length];
                for (var i = 0; i < this.transparentMaterials.Length; i++)
                {
                    this.transparentMaterials[i] = transparentMaterial;
                }
            }
        }

        public HeroBase(Transform transform, int hero, float attackHeight = 1.5f)
        {
            this.id = hero;
            
            this.attackHeight = attackHeight;
            this.nametagHeight = 2.6f;

            this.staticHero = StaticHeroDataManager.Get().GetData(hero);
            
            this.assetBundle = AssetManager.LoadAssetBundle0(this.staticHero.bundle);
            var model = this.LoadAsset(this.staticHero.model);

            var heroObject = Object.Instantiate(model, transform);

            this.GetRenderer(heroObject.transform);
            heroObject.GetComponent<LobbyHeroBase>().blinkEnabled = false;

            this.animator = heroObject.GetComponent<Animator>();
        }

        private void GetRenderer(Transform transform)
        {
            Material transparentMaterial = null;

            foreach (Transform t in transform)
            {
                if (t.TryGetComponent<Renderer>(out var renderer))
                {
                    if (!transparentMaterial)
                    {
                        var mat = renderer.material;
                        var tex = mat.mainTexture;

                        transparentMaterial = new Material(GameManager.instance.heroMatTransparent);
                        transparentMaterial.mainTexture = tex;
                    }

                    this.renderers.Add(new RendererInfo(transparentMaterial, renderer));
                }

                this.GetRenderer(t);
            }
        }

        public virtual void Tick()
        {
            ++this.lastAttack;
        }
        
        public abstract void Render(float partialTicks);

        public virtual void OnPreAttack(float x, float y, float rot, Vector2 vec)
        {
            this.lastAttack = 0;
            this.animator.SetTrigger("attack");
        }

        public void SetRunning(bool running)
        {
            this.animator.SetBool("run", running);
        }

        public void MakeTransparent()
        {
            foreach (var renderer in this.renderers)
            {
                renderer.renderer.sharedMaterials = renderer.transparentMaterials;
            }
        }

        public void MakeVisible()
        {
            foreach (var renderer in this.renderers)
            {
                renderer.renderer.sharedMaterials = renderer.originalMaterials;
            }
        }

        public GameObject LoadParticle(int index)
        {
            return Object.Instantiate(this.LoadAsset(this.staticHero.particles[index]));
        }

        public GameObject LoadAsset(string path)
        {
            return this.assetBundle.LoadAsset<GameObject>(path);
        }
    }
}