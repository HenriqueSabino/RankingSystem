using RankingSystem.model.Dao.Impl;
using RankingSystem.db;

namespace RankingSystem.model.Dao
{
    public static class DaoFactory
    {
        // class used to create Dao's of certain implementations
        public static PlayerDao createPlayerDao()
        {
            return new PlayerDaoMySqlData(DB.GetConnection());
        }

        public static PlayerRecordsDao createPlayerRecordsDao()
        {
            return new PlayerRecordsDaoMySqlData(DB.GetConnection());
        }
    }
}