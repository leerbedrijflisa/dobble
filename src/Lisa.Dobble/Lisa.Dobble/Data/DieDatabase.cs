using Lisa.Dobble.Models;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;
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

        public void RemoveDice()
        {
            database.DeleteAll<Die>();
        }

        public IEnumerable<Die> GetDice()
        {
            return database.GetAllWithChildren<Die>();
            //return (from i in database.Table<Die>() select i).ToList();
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

        public int InsertOption(Option option)
        {
            return database.Insert(option);
        }

        public void InsertDie(Die die)
        {
            database.InsertWithChildren(die);
        }

        public Die GetDie(int dieId)
        {
            return database.GetWithChildren<Die>(dieId);
        }
    }
}
