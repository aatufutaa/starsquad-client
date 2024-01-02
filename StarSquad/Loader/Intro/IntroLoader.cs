using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StarSquad.Loader.Intro
{
    public class IntroLoader : MonoBehaviour
    {
        public Animator animator;
        public AudioSource sound;

        private bool finished;

        private void Awake()
        {
            // if not muted play sound
            PreLoader.Init();
            if (PreLoader.sounds)
            {
                this.sound.Play();
            }
        }

        private IEnumerator StartLoadScene()
        {
            this.animator.gameObject.SetActive(false);
            
            var op = SceneManager.LoadSceneAsync("Local/Loader/Loader", LoadSceneMode.Additive);
            
            yield return op;
            //yield return null;
            //yield return null;
            
            //this.LoadScene();
            
            var scene = SceneManager.GetSceneByName("Local/Loader/Loader");
            SceneManager.SetActiveScene(scene);
        }

        private void LoadScene()
        {
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

            var op = SceneManager.LoadSceneAsync("Local/Loader/Loader", LoadSceneMode.Additive);
        }
        
        private void Update()
        {
            if (this.finished) return;
            
            var state = this.animator.GetCurrentAnimatorStateInfo(0);
            if (state.normalizedTime < state.length) return;

            this.finished = true;

            StartCoroutine(this.StartLoadScene());
        }
    }
}