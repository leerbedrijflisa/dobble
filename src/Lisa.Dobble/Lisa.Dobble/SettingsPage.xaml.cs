using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lisa.Dobble
{
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
           
            var s = new SegmentControl();
            s.TintColor = Color.Red;
            s.AddSegment("Geheel scherm");
            s.HeightRequest = 30;
            s.AddSegment("Dobbelsteen");
            s.SelectedSegmentChanged += (object sender, int segmentIndex) =>
            {
                s.TintColor = Color.Red;
            };
            s.SelectedSegment = 1;
            SettingsGrid.Children.Add(s, 1, 3);

            StartButton.Clicked +=StartButton_Clicked;
        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {

            Navigation.PushAsync(new Main());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
