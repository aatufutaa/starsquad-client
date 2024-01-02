using StarSquad.Common.Hero;
using StarSquad.Loader;
using StarSquad.Loader.Asset;
using StarSquad.Lobby.UI.Util;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace StarSquad.Lobby.Hero
{
    public class HeroPlayer : MonoBehaviour
    {
        public RawImage mainTarget;
        public RawImage heroScreenTarget;

        public CustomVideoPlayer customVideoPlayer;
        
        public void Play(int id, bool target)
        {
            var staticHero = StaticHeroDataManager.Get().GetData(id);

            var bundle = AssetManager.LoadAssetBundle0(staticHero.bundle);
            var clip = bundle.LoadAsset<VideoClip>(staticHero.clip);
            
            if (!clip)
            {
                LoaderManager.instance.ShowBadError("Hero failed to load");
                return;
            }
            
            this.customVideoPlayer.target = target ? this.mainTarget : this.heroScreenTarget;
            this.customVideoPlayer.PlayVideo(clip);
        }

        public static HeroPlayer Get()
        {
            return LobbyManager.instance.heroPlayer;
        }
    }
}