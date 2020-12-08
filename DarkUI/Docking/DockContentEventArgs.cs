using System;

namespace DarkUI.Docking
{
    public class DockContentEventArgs : EventArgs
    {
        public DockContentEventArgs(DarkDockContent content)
        {
            Content = content;
        }

        public DarkDockContent Content { get; }
    }
}