using System;
using System.Collections;
using StarSquad.Common.Hero;
using StarSquad.Common.Level;
using StarSquad.Game.Misc;
using StarSquad.Loader.Asset;
using StarSquad.Loader.Dialog;
using StarSquad.Loader.Intro;
using StarSquad.Lobby;
using StarSquad.Lobby.UI.Util;
using StarSquad.Net;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StarSquad.Loader
{
    public class LoaderManager : MonoBehaviour
    {
        public static LoaderManager instance;

        // loading screen
        public CanvasGroup canvasGroup;
        public AudioSource sound;
        public Image progressFill;
        public TMP_Text progressText;
        private Material fill;
        public TMP_Text subText;

        // loader
        private int loaderStage;
        private float timer;
        private Action loadCallback;
        private bool downloadingContent;
        [NonSerialized] public float downloadingProgress;
        public AssetManager assetManager;
        [NonSerialized] public float subProgress;
        public BadInternet badInternet;

        // general
        public MainTaskQueue mainTaskQueue;
        public NetworkManager networkManager;
        [NonSerialized] public AlertManager alertManager;
        [NonSerialized] public NativeDialogManager nativeDialogManager;

        public StaticHeroDataManager heroDataManager;
        public StaticLevelManager levelManager;
        public StaticShopManager shopManager;

        private bool stopped;

        public void Awake()
        {
#if UNITY_EDITOR
            PreLoader.Init(); // in case this scene was loader in editor
#endif

            instance = this;

            if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Application.targetFrameRate = 60;
            }
            Application.targetFrameRate = 60;

            this.fill = MaterialUtil.DuplicateMaterial(this.progressFill);

            this.ShowLoader(true, () =>
            {
                Debug.Log("Initializing!!!");

                PreLoader.LoadSettings();

                this.badInternet = Instantiate(Resources.Load<BadInternet>("BadInternet"), this.transform);
                this.badInternet.transform.SetAsFirstSibling();
                
                this.nativeDialogManager = new GameObject("DialogManager").AddComponent<NativeDialogManager>();
                this.nativeDialogManager.transform.SetParent(this.transform);

                Instantiate(Resources.Load<GameObject>("Debug"), this.transform);

                this.alertManager = Instantiate(Resources.Load<AlertManager>("AlertManager"), this.transform);

                this.gameObject.AddComponent<EventSystem>();
                this.gameObject.AddComponent<InputSystemUIInputModule>();

                this.alertManager.Init(); // init alert list

                DontDestroyOnLoad(this.transform); // dont remove loader on scene load

                this.mainTaskQueue = new MainTaskQueue();
                this.networkManager = new NetworkManager();

                Debug.Log("Loader done");
            });
        }

        private void Stop()
        {
            this.stopped = true;

            try
            {
                this.networkManager?.Stop();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }

            instance = null;
        }

        private void Update()
        {
            if (this.stopped) return;

            switch (this.loaderStage)
            {
                case 0: // not loading / loading finished
                    return; // dont update progress either

                case 1: // first -> fade in
                case 10: // last -> fade out
                    this.timer += Time.deltaTime;

                    var fadeIn = this.loaderStage == 1;
                    var keepScreenTime = fadeIn ? 0f : 0.4f;
                    if (this.timer > keepScreenTime)
                    {
                        const float fadeTime = 0.4f;
                        var p = (this.timer - keepScreenTime) / fadeTime;

                        if (p > 1f)
                        {
                            p = 1f;

                            if (fadeIn)
                            {
                                this.loaderStage = 2;
                            }
                            else
                            {
                                this.loaderStage = 0;
                                this.canvasGroup.gameObject.SetActive(false);
                            }
                        }

                        if (!fadeIn)
                            p = 1f - p;

                        this.canvasGroup.alpha = p;
                    }

                    break;

                case 2: // load callback
                    this.loaderStage = 3;

                    this.loadCallback.Invoke();
                    break;
            }

            float progress;
            if (this.downloadingContent)
            {
                progress = this.downloadingProgress;
            }
            else
            {
                progress = (this.loaderStage + this.subProgress) / 10f;
            }

            this.progressText.text = Mathf.RoundToInt(progress * 100) + "%";
            ProgressBarUtil.SetProgress(this.fill, progress);
        }

        public void SetDownloading()
        {
            this.subText.gameObject.SetActive(true);
            this.subText.text = "Downloading content...";
            this.downloadingContent = true;
        }

        private void FixedUpdate()
        {
            if (this.stopped) return;

            this.mainTaskQueue?.ProcessTaskQueue();

            this.networkManager?.Tick();
        }

        public void ShowLoader(bool playSound, Action loadCallback)
        {
            // if show loader while already shown
            if (this.loaderStage > 0)
            {
                loadCallback.Invoke();
                return;
            }

            //this.camera.gameObject.SetActive(true);

            this.loaderStage = 1;
            this.canvasGroup.alpha = 0;
            this.timer = 0f;

            this.canvasGroup.gameObject.SetActive(true);

            if (playSound && PreLoader.sounds)
                this.sound.Play();

            this.loadCallback = loadCallback;
        }

        public void HideLoader()
        {
            this.loaderStage = 10;
            this.timer = 0f;
            
            PlayerAuthentication.LoginToGameCenter();
        }

        public void UpdateStage(int stage)
        {
            if (stage < this.loaderStage) return; // dont allow go back
            this.loaderStage = stage;
        }

        private IEnumerator StartReload()
        {
            Debug.Log("Reloading...");

            // TODO: clear active dialo
            
            try
            {
                this.Stop();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            yield return SceneManager.LoadSceneAsync("Local/Loader/Empty");

            yield return null;

            this.assetManager?.UnloadAllAssetBundles();

            yield return Resources.UnloadUnusedAssets();

            Destroy(this.gameObject); // destroy loader

            yield return SceneManager.LoadSceneAsync("Local/Loader/Intro/Intro");
        }

        public void Reload()
        {
            StartCoroutine(this.StartReload());
        }

        private void OnApplicationQuit()
        {
            this.Stop();
        }

        public static bool IsUsingNet()
        {
            return !ReferenceEquals(instance, null);
        }

        public void ShowBadError(string msg)
        {
            this.networkManager?.connectionManager.SafeDisconnect("bad error");

            this.nativeDialogManager.ShowDialog("Game Error", msg, "Try again", (button) => { this.Reload(); });
        }

        public void OnAssetsLoaded()
        {
            this.networkManager.sessionManager.EnableEncryption();

            this.heroDataManager = new StaticHeroDataManager();
            this.heroDataManager.Load();

            this.levelManager = new StaticLevelManager();
            this.levelManager.Load();

            this.shopManager = new StaticShopManager();
            this.shopManager.Load();
        }

        public void PauseGame()
        {
            Time.timeScale = 0f;

            var lobby = LobbyManager.instance;
            if (lobby)
            {
                lobby.PauseLobby();
            }
            
            // TODO: pause audio
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            
            var lobby = LobbyManager.instance;
            if (lobby)
            {
                lobby.ResumeLobby();
            }
            
            // TODO: resume audio
        }
    }
}