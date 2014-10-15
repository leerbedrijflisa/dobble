using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lisa.Dobble
{
    public partial class Main
    {
        public Main()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            BackButton.Clicked += BackButton_Clicked;
        }

        void BackButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void ButtonOnClick(object sender, EventArgs e)
        {
           
        }
    }
}
