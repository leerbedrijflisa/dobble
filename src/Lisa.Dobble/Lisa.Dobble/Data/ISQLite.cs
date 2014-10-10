using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisa.Dobble.Data
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
