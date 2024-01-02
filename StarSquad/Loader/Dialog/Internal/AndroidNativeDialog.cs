#if UNITY_ANDROID

using UnityEngine;

namespace StarSquad.Loader.Dialog.Internal
{
    public class AndroidNativeDialog : Dialog
    {
        private readonly AndroidJavaClass clazz;

        public AndroidNativeDialog()
        {
            this.clazz = new AndroidJavaClass("");
        }

        public void Dispose()
        {
            this.clazz.Dispose();
        }

        public void ShowDialog(int id, string title, string msg, string button)
        {
            this.clazz.CallStatic("showDialog", id, title, msg, button);
        }
    }
}

#endif // UNITY_ANDROID