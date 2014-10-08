using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Lisa.Dobble
{
    public class App
    {
        public static Page GetMainPage()
        {
            var settingsPage = new NavigationPage(new SettingsPage());
           

            return settingsPage;
        }
    }
}
