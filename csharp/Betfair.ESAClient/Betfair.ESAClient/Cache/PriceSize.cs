using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betfair.ESAClient.Cache
{
    /// <summary>
    /// Immutable tuple of price, size
    /// </summary>
    public class PriceSize
    {
        private readonly decimal _price;
        private readonly decimal _size;
        public static readonly IList<PriceSize> EmptyList = new PriceSize[0];

        public PriceSize(List<decimal?> priceSize)
        {
            _price = (decimal)priceSize[0];
            _size = (decimal)priceSize[1];
        }

        public PriceSize(decimal price, decimal size)
        {
            _price = price;
            _size = size;
        }

        public decimal Price
        {
            get
            {
                return _price;
            }
        }

        public decimal Size
        {
            get
            {
                return _size;
            }
        }

        public override string ToString()
        {
            return _size + "@" + _price;
        }
    }
}
