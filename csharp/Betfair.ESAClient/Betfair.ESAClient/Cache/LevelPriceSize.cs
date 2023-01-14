using System.Collections.Generic;

namespace Betfair.ESAClient.Cache {
    /// <summary>
    /// Immutable triple of level, price size.
    /// </summary>
    public class LevelPriceSize {
        private readonly int _level;
        private readonly decimal _price;
        private readonly decimal _size;
        public static readonly IList<LevelPriceSize> EmptyList = new LevelPriceSize[0];

        public LevelPriceSize(List<decimal?> levelPriceSize) {
            _level = (int) levelPriceSize[0];
            _price = (decimal) levelPriceSize[1];
            _size = (decimal) levelPriceSize[2];
        }

        public LevelPriceSize(int level, decimal price, decimal size) {
            _level = level;
            _price = price;
            _size = size;
        }

        public int Level {
            get { return _level; }
        }

        public decimal Price {
            get { return _price; }
        }

        public decimal Size {
            get { return _size; }
        }

        public override string ToString() {
            return _level + ": " + _size + "@" + _price;
        }
    }
}
