namespace RankingSystem.db
{
    [System.Serializable]
    public class DBIntegrityException : System.Exception
    {
        // this exception type is used to wrap the MySql.Data, this way it doesn't need to be imported
        // every where and when the problem is an integrity error
        public DBIntegrityException() { }
        public DBIntegrityException(string message) : base(message) { }
        public DBIntegrityException(string message, System.Exception inner) : base(message, inner) { }
        protected DBIntegrityException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}