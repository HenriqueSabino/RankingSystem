using System.Collections.Generic;
using RankingSystem.model.entities;

namespace RankingSystem.model.Dao
{
    public interface PlayerRecordsDao
    {
        void Insert(PlayerRecords obj);

        void Update(PlayerRecords obj);

        void DeleteById(int id);

        PlayerRecords FindById(int id);

        List<PlayerRecords> FindAll();
    }
}