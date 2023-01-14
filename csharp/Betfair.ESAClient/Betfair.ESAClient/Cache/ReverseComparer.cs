using System.Collections.Generic;

namespace Betfair.ESAClient.Cache
{
    public class ReverseComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> _comparer;

        public ReverseComparer(IComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return _comparer.Compare(y, x);
        }
    }
}
