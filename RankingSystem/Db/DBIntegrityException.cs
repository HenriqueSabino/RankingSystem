namespace RankingSystem.db
{
    [System.Serializable]
    public class DBIntegrityException : System.Exception
    {
        public DBIntegrityException() { }
        public DBIntegrityException(string message) : base(message) { }
        public DBIntegrityException(string message, System.Exception inner) : base(message, inner) { }
        protected DBIntegrityException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}