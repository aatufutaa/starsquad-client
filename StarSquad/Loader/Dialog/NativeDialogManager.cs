using System;
using System.Collections.Generic;
using StarSquad.Loader.Dialog.Internal;
using UnityEngine;

namespace StarSquad.Loader.Dialog
{
    public class NativeDialogManager : MonoBehaviour
    {
        private Internal.Dialog dialog;

        private int id;

        private readonly Dictionary<int, Action<int>> listener = new();

        public void Start()
        {
            this.dialog = CreateDialog();
        }

        public void OnDestroy()
        {
            this.dialog.Dispose();
        }

        private static Internal.Dialog CreateDialog()
        {
#if UNITY_EDITOR
            return new FallbackNativeDialog();
#elif UNITY_ANDROID
            return new AndroidNativeDialog();
#elif UNITY_IOS
            return new IosNativeDialog();
#else
            return new FallbackNativeDialog();
#endif
        }

        private int dialogCount;
        
        public void ShowDialog(string title, string msg, string button, Action<int> onClick)
        {
            var id = this.id++;
            this.listener.Add(id, onClick);
            this.dialog.ShowDialog(id, title, msg, button);

            ++this.dialogCount;
            LoaderManager.instance.PauseGame();
        }

        public void HandleResponse(string response)
        {
            --this.dialogCount;
            if (this.dialogCount <= 0)
                LoaderManager.instance.ResumeGame();
            
            Debug.Log("response " + response);

            var parts = response.Split(",");
            var id = int.Parse(parts[0]);
            var button = int.Parse(parts[1]);

            if (!this.listener.TryGetValue(id, out var val)) return;
            this.listener.Remove(id);
            val(button);
        }
    }
}