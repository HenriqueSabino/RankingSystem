using System.Collections.Generic;
using RankingSystem.model.Dao;
using RankingSystem.model.entities;

namespace RankingSystem.model.Services
{
    public class PlayerService
    {
        private PlayerDao playerDao = DaoFactory.createPlayerDao();

        public Player FindByUserName(string userName)
        {
            return playerDao.FindByUserName(userName);
        }

        public List<Player> FindAll()
        {
            return playerDao.FindAll();
        }

        public void SaveOrUpdate(Player obj)
        {
            if (obj.Id == null)
            {
                playerDao.Insert(obj);
            }
            else
            {
                playerDao.Update(obj);
            }
        }

        public void Remove(Player obj)
        {
            if (obj.Id.HasValue)
                playerDao.DeleteById(obj.Id.Value);
        }
    }
}