using System.Collections.Generic;

namespace Zwyssigly.ImageServer.Contracts
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
