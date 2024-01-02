using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StarSquad.Loader.Intro
{
    public class IntroPlayer : MonoBehaviour
    {
        public Sprite[] sprites;
        public int fps = 4;

        public Image image;

        private float frameTimer;
        private float timer;

        private int frame;
        
        public AudioSource sound;

        private bool finished;
        
        private void Awake()
        {
            this.frameTimer = 1f / this.fps;
            
            // if not muted play sound
            PreLoader.Init();
            if (PreLoader.sounds)
            {
                this.sound.Play();
            }
        }

        private void Update()
        {
            if (this.finished) return;
            
            this.timer += Time.deltaTime;
            if (this.timer < this.frameTimer) return;
                                                   
                this.timer -= this.frameTimer;
                ++this.frame;

                if (this.frame > this.sprites.Length)
                {
                    this.finished = true;
                    
                    try
                    {
                        var key = PlayerPrefs.GetString("loader");
                        if (key.Length > 0)
                        {
                            var bundle = AssetBundle.LoadFromFile(key);
                            SceneManager.LoadScene(bundle.GetAllScenePaths()[0]);
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }

                    SceneManager.LoadScene("Local/Loader/Loader");
                    
                } else if (this.frame == this.sprites.Length)
                {
                    this.transform.parent.gameObject.SetActive(false);
                }                                              else
                {
                    this.image.sprite = this.sprites[this.frame];
                }
            
        }
    }
}