using System.Collections.Generic;
using RankingSystem.model.Dao;
using RankingSystem.model.entities;

namespace RankingSystem.model.Services
{
    public class PlayerRecordsService
    {
        private PlayerRecordsDao playerRecordsDao = DaoFactory.createPlayerRecordsDao();

        public PlayerRecords FindById(int id)
        {
            return playerRecordsDao.FindById(id);
        }

        public List<PlayerRecords> FindAll()
        {
            return playerRecordsDao.FindAll();
        }

        public void SaveOrUpdate(PlayerRecords obj)
        {
            if (obj.LastUpdated == null)
            {
                playerRecordsDao.Insert(obj);
            }
            else
            {
                playerRecordsDao.Update(obj);
            }
        }

        public void Remove(PlayerRecords obj)
        {
            if (obj.PlayerId.HasValue)
                playerRecordsDao.DeleteById(obj.PlayerId.Value);
        }
    }
}