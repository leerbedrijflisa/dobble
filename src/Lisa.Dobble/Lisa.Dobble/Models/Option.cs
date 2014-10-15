using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lisa.Dobble.Models
{
    public class Option
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Die))]
        public int DieId { get; set; }
        public string Image { get; set; }
        public string Sound { get; set; }

        [ManyToOne]
        public Die Die { get; set; }
    }
}
