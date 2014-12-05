using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisa.Dobble.Models
{
    public class Die
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)] 
        public List<Option> Options { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPremium { get; set; }
    }
}
