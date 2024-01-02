using UnityEngine;

namespace StarSquad.Lobby.UI.Util
{
    public class MathHelper
    {
        public static float SmoothLerp(float v)
        {
            return Mathf.Sin(v * Mathf.PI / 2f);
        }
    }
}