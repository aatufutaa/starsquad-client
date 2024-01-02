using UnityEngine;

namespace StarSquad.Game.Misc
{
    public class AngleUtil
    {
        public const float Angle = 53.13f;
        
        private static readonly Quaternion rot = Quaternion.Euler(Angle, 0f, 0f);
        private static readonly Vector3 dir = rot * Vector3.forward;

        public const int RingOffset = 5;
        
        public static void FixAngle(Transform transform, int offset)
        {
            transform.localRotation = rot;
            transform.localPosition = dir * offset;
        }
    }
}