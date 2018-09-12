using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace F1TelemetryUi.Utility
{
    public class SortedObservableCollection<T> : ObservableCollection<T>
    {
        public SortedObservableCollection() : base()
        {

        }

        public SortedObservableCollection(IEnumerable<T> items) : base(items)
        {

        }
        /// <summary>
        /// Sorts the items in the collection using the provided key selector.
        /// </summary>
        /// <typeparam name="TKey">Key type returned by the key selector.</typeparam>
        /// <param name="selector">Function to retrieve the key from an item.</param>
        public void Sort<TKey>(Func<T, TKey> selector)
        {
            Sort(Items.OrderBy(selector));
        }

        /// <summary>
        /// Sorts the items in the collection using the provided key selector.
        /// </summary>
        /// <typeparam name="TKey">Key type returned by the key selector.</typeparam>
        /// <param name="selector">Function to retrieve the key from an item.</param>
        /// <param name="comparer">A <see cref="IComparer{T}"/> to compare keys.</param>
        public void Sort<TKey>(Func<T, TKey> selector, IComparer<TKey> comparer)
        {
            Sort(Items.OrderBy(selector, comparer));
        }

        /// <summary>
        /// Moves items in the inner collection to match the positions of the items provided.
        /// </summary>
        /// <param name="items">
        /// A <see cref="IEnumerable{T}"/> to provide the positions of the items.
        /// </param>
        private void Sort(IEnumerable<T> items)
        {
            List<T> itemsList = items.ToList();

            foreach (T item in itemsList)
            {
                Move(IndexOf(item), itemsList.IndexOf(item));
            }
        }
    }
}
