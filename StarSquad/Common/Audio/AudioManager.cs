using System;
using System.Collections.Generic;
using StarSquad.Loader.Intro;
using UnityEngine;

namespace StarSquad.Common.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;
        }

        public Sound[] sounds;
        public Sound[] music;

        private readonly Dictionary<string, AudioSource> loadedSounds = new();
        private readonly Dictionary<string, AudioSource> loadedMusic = new();

        private void Awake()
        {
            foreach (var sound in this.sounds)
                this.LoadSound(sound, this.loadedSounds);
            foreach (var music in this.music)
                this.LoadSound(music, this.loadedMusic);

            if (!PreLoader.sounds) this.MuteSounds(true);
            if (!PreLoader.music) this.MuteMusic(true);
        }

#if UNITY_EDITOR
        public void Refresh()
        {
            this.MuteSounds(!PreLoader.sounds);
            this.MuteMusic(!PreLoader.music);
        }
#endif

        private void LoadSound(Sound sound, IDictionary<string, AudioSource> dictionary)
        {
            var audioSource = new GameObject(sound.name).AddComponent<AudioSource>();
            audioSource.transform.SetParent(this.transform);
            audioSource.clip = sound.clip;
            dictionary.Add(sound.name, audioSource);
        }

        public void PlaySound(string name)
        {
            if (this.loadedSounds.TryGetValue(name, out var sound))
                sound.Play();
        }

        public void PlayMusic(string name, bool looping)
        {
            if (this.loadedMusic.TryGetValue(name, out var sound))
            {
                sound.Play();
                sound.loop = looping;
            }
        }

        public void OnSoundsEnabled(bool enabled)
        {
            PreLoader.sounds = enabled;
            PlayerPrefs.SetInt("mute_sounds", enabled ? 0 : 1);
            this.MuteSounds(!enabled);
        }

        private void MuteSounds(bool mute)
        {
            foreach (var audioSource in this.loadedSounds.Values)
            {
                audioSource.mute = mute;
            }
        }

        public void OnMusicEnabled(bool enabled)
        {
            PreLoader.music = enabled;
            PlayerPrefs.SetInt("mute_music", enabled ? 0 : 1);
            this.MuteMusic(!enabled);
        }

        private void MuteMusic(bool mute)
        {
            foreach (var audioSource in this.loadedMusic.Values)
            {
                audioSource.mute = mute;
            }
        }
        
        public void MuteAll()
        {
            if (PreLoader.music)
            {
                this.MuteMusic(true);
            }
            
            if (PreLoader.sounds)
            {
                this.MuteSounds(true);
            }
        }

        public void UnmuteAll()
        {
            if (PreLoader.music)
            {
                this.MuteMusic(false);
            }
            
            if (PreLoader.sounds)
            {
                this.MuteSounds(false);
            }
        }
    }
}