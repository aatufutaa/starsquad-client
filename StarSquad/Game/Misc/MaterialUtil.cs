using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Game.Misc
{
    public class MaterialUtil
    {
        public static Material DuplicateMaterial(Image image)
        {
            var mat = new Material(image.material);
            image.material = mat;
            return mat;
        }
    }
}