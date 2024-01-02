using UnityEngine;
using Object = UnityEngine.Object;

namespace StarSquad.Loader.Dialog.Internal
{
    public class FallbackNativeDialog : Dialog
    {
        private readonly FallbackDialog prefab;

        public FallbackNativeDialog()
        {
            this.prefab = Resources.Load<FallbackDialog>("FallbackDialog");
        }

        public void Dispose()
        {
        }

        public void ShowDialog(int id, string title, string msg, string button)
        {
            var dialog = Object.Instantiate(this.prefab, LoaderManager.instance.transform);
            dialog.SetId(id);
            dialog.SetTitle(title);
            dialog.SetMsg(msg);
            dialog.AddButton(button);
        }
    }
}