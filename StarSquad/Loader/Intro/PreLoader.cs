using UnityEngine;

namespace StarSquad.Loader.Intro
{
    public class PreLoader
    {
#if UNITY_EDITOR
        private static bool initialized;
#endif
        public static bool sounds;
        public static bool music;

        public static void Init()
        {
#if UNITY_EDITOR
            if (initialized) return;
            initialized = true;
#endif
            sounds = PlayerPrefs.GetInt("mute_sounds", 0) == 0;
        }

        public static void LoadSettings()
        {
            music = PlayerPrefs.GetInt("mute_music", 0) == 0;
        }
    }
}