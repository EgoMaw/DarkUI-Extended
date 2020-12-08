using System;

namespace DarkUI.Controls
{
    public class ScrollValueEventArgs : EventArgs
    {
        public ScrollValueEventArgs(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}