using UnityEngine;

namespace StarSquad.Lobby.UI.Util
{
    public class ProgressBarUtil
    {
        public static void SetProgress(Material fill, float p)
        {
            fill.SetFloat("_Progress", Mathf.Clamp01(p));
        }
    }
}