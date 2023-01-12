using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betfair.ESAClient.Cache
{
    public class Utils
    {
        public static decimal SelectPrice(bool isImage, ref decimal currentPrice, decimal? newPrice)
        {
            if (isImage)
            {
                currentPrice = newPrice ?? 0.0M;
            }
            else
            {
                currentPrice = newPrice ?? currentPrice;
            }
            return currentPrice;
        }
    }
}
