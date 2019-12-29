using System;
using System.Runtime.Serialization;

namespace RankingSystem.db
{
    [System.Serializable]
    public class DBException : Exception
    {
        public DBException() { }
        public DBException(string message) : base(message) { }
        public DBException(string message, Exception inner) : base(message, inner) { }
        protected DBException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}