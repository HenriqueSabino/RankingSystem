using System.Collections.Generic;
using RankingSystem.model.entities;

namespace RankingSystem.model.Dao
{
    public interface PlayerDao
    {
        // model of what need to be implemented
        void Insert(Player obj);

        void Update(Player obj);

        void DeleteById(int id);

        Player FindById(int id);

        Player FindByUserName(string name);

        List<Player> FindAll();
    }
}

