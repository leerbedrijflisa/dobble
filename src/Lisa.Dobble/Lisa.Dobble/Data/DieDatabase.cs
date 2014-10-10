using Lisa.Dobble.Models;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lisa.Dobble.Data
{
    class DieDatabase
    {
        SQLiteConnection database;

        public DieDatabase()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<Die>();
            database.CreateTable<Option>();
        }

        public IEnumerable<Die> GetDice()
        {
            return (from i in database.Table<Die>() select i).ToList();
        }

        public Die GetDie(int id)
        {
            return database.Table<Die>().FirstOrDefault(x => x.Id == id);
        }

        public int SaveDie(Die die)
        {
            if(die.Id != 0)
            {
                database.Update(die);
                return die.Id;
            }
            else
            {
                return database.Insert(die);
            }
        }

        public int DeleteDie(int id)
        {
            return database.Delete<Die>(id);
        }
    }
}
