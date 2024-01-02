using System;

namespace StarSquad.Loader.Dialog.Internal
{
    public interface Dialog : IDisposable
    {
        void ShowDialog(int id, string title, string msg, string button);
    }
}