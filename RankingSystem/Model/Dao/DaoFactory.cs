using RankingSystem.model.Dao.Impl;
using RankingSystem.db;

namespace RankingSystem.model.Dao
{
    public static class DaoFactory
    {
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