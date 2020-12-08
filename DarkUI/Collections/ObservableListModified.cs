using System;
using System.Collections.Generic;

namespace DarkUI.Collections
{
    public class ObservableListModified<T> : EventArgs
    {
        public ObservableListModified(IEnumerable<T> items)
        {
            Items = items;
        }

        public IEnumerable<T> Items { get; }
    }
}