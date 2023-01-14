namespace Betfair.ESAClient.Cache
{
    public class RunnerId 
    {
        private readonly long _selectionId;
        private readonly decimal? _handicap;

        public RunnerId(long? selectionId, decimal? handicap)
        {
            _selectionId = (long)selectionId;
            _handicap = handicap;
        }

        public long SelectionId
        {
            get
            {
                return _selectionId;
            }
        }

        public decimal? Handicap
        {
            get
            {
                return _handicap;
            }
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            RunnerId runnerId = (RunnerId)obj;

            if (_selectionId != runnerId._selectionId) return false;
            return _handicap == runnerId._handicap;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            int result = (int)(_selectionId ^ (_selectionId >> 32));
            result = 31 * result + (_handicap != null ? _handicap.GetHashCode() : 0);
            return result;
        }

        public override string ToString()
        {
            return _handicap == null ? _selectionId.ToString() : _selectionId + ":" + _handicap;
        }
    }
}
