using System;
using System.Runtime.Serialization;

namespace RankingSystem.db
{
    [System.Serializable]
    public class DBException : Exception
    {
        // this exception type is used to wrap the MySql.Data, this way it doesn't need to be imported
        // every where
        public DBException() { }
        public DBException(string message) : base(message) { }
        public DBException(string message, Exception inner) : base(message, inner) { }
        protected DBException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}