using System;
using System.Collections.Generic;
using System.Text;

namespace Zwyssigly.ImageServer
{
    public class Page<T>
    {
        public IReadOnlyCollection<T> Items { get; }
        public ulong TotalItems { get; }

        public Page(IReadOnlyCollection<T> items, ulong totalItems)
        {
            Items = items;
            TotalItems = totalItems;
        }
    }
}
