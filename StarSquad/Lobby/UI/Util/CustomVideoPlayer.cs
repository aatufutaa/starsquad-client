using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;

namespace StarSquad.Lobby.UI.Util
{
    public class CustomVideoPlayer : MonoBehaviour
    {
        public RawImage target;

        private VideoPlayer videoPlayer;

        private bool blink;
        private float nextBlink;
        
        private bool init;
        
        private bool loaded;
        private bool render;
        private bool seeking;

        private bool skipNext;

        private bool pause;

        private void Awake()
        {
            this.Init();
        }

        public bool IsLoaded()
        {
            return this.videoPlayer.isPrepared && !this.next;
        }

        public void Init()
        {
            if (this.init) return;
            this.init = true;
            this.videoPlayer = this.GetComponent<VideoPlayer>();
            this.videoPlayer.prepareCompleted += this.OnPrepared;
            //this.videoPlayer.seekCompleted += this.OnSeekDone;
            this.videoPlayer.loopPointReached += this.OnVideoEnded;
        }

        private void OnVideoEnded(VideoPlayer videoPlayer)
        {
            this.videoPlayer.Play();
            if (this.skipNext)
            {
                this.videoPlayer.frame = 39;
                this.skipNext = false;
            }
            else
            {
                this.videoPlayer.frame = 0;
                this.skipNext = Random.Range(0f,1f) > 0.5f;
            }
        }

        private void OnSeekDone(VideoPlayer videoPlayer)
        {
            this.seeking = false;
        }
        
        private void OnPrepared(VideoPlayer videoPlayer)
        {
            this.target.texture = this.videoPlayer.texture;
            this.target.SetNativeSize();

            this.loaded = true;
            
            if (!this.pause)
            this.videoPlayer.Play();

            this.next = true;

            //this.PlayNextFrame();
        }

        private void FixedUpdate()
        {
            //if (!this.loaded) return;
            //if (this.render) return;

            //this.PlayNextFrame();
        }

        /*private void PlayNextFrame()
        {
            if (this.seeking) return;
            
            var lastFrame = this.videoPlayer.frame;
            if (lastFrame == (long)this.videoPlayer.frameCount - 1)
            {
                this.videoPlayer.frame = 0;

                if (Time.time > this.nextBlink)
                {
                    this.blink = true;
                    this.nextBlink = Time.time + Random.Range(1.5f, 5f);
                }
            }
            else if (lastFrame == 0 && !this.blink)
            {
                this.videoPlayer.frame = 6;
            }
            else if (this.blink && lastFrame == 5)
            {
                this.videoPlayer.frame = 5 + 5 + 1;
                this.blink = false;
            }
            else
            {
                this.videoPlayer.frame = lastFrame + 1;
            }

            this.videoPlayer.Play();
            this.videoPlayer.Pause();

            this.render = true;

            this.seeking = true;
        }*/

        private bool next;

        private void Update()
        {
            if (this.next)
            {
                this.next = false;
                this.target.color = new Color(1f, 1f, 1f, 1f);
            }
            
            this.render = false;
        }

        public void StopVideo()
        {
            this.loaded = false;
            this.videoPlayer.Stop();
        }

        public void PlayVideo(VideoClip clip)
        {
            this.loaded = false;

            this.target.color = new Color(1f, 1f, 1f, 0f);
            this.next = false;

            if (this.videoPlayer.clip == clip && this.videoPlayer.isPrepared)
            {
                this.OnPrepared(this.videoPlayer);
                return;
            }
            
            this.videoPlayer.clip = clip;
            this.videoPlayer.Prepare();
        }

        public void PausePlayer()
        {
            this.pause = true;
            this.videoPlayer.Pause();
        }

        public void ResumePlayer()
        {
            this.pause = false;
            this.videoPlayer.Play();
        }
    }
}