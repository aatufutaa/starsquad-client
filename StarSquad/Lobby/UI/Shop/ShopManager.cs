using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Shop
{
    public class ShopManager : MonoBehaviour
    {
        public ScrollRect scrollRect;

        public void ShowGems()
        {
            this.scrollRect.verticalNormalizedPosition = 0f;
        }
        
        public static ShopManager Get()
        {
            return LobbyManager.instance.shopManager;
        }
    }
}