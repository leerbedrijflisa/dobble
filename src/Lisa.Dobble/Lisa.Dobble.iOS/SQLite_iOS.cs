using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Lisa.Dobble.Data;
using System.IO;
using Xamarin.Forms;
using Lisa.Dobble.iOS;

[assembly: Dependency(typeof(SQLite_iOS))]
namespace Lisa.Dobble.iOS
{
    public class SQLite_iOS : ISQLite
    {
        public SQLite_iOS() { }
        public SQLite.Net.SQLiteConnection GetConnection()
        {
            var sqliteFilename = "DieDatabase.db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            var path = Path.Combine(libraryPath, sqliteFilename);
            // Create the connection
            var plat = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
            var conn = new SQLite.Net.SQLiteConnection(plat, path);
            // Return the database connection
            return conn;
        }
    }
}