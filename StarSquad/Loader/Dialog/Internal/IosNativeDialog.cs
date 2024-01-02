#if UNITY_IOS

using System.Runtime.InteropServices;

namespace StarSquad.Loader.Dialog.Internal
{
    public class IosNativeDialog : Dialog
    {
        public void Dispose(){}

        public void ShowDialog(int id, string title, string msg, string button)
        {
            _ShowDialog(id, title, msg, button);
        }

        [DllImport("__Internal")]
        private static extern void _ShowDialog(int id, string title, string msg, string button);
    }
}

#endif // UNITY_IOS