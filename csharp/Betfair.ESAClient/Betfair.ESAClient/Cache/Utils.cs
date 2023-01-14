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
