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
            InitializeAdditionalComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            StartButton.Clicked +=StartButton_Clicked;
        }

        private void InitializeAdditionalComponent()
        {
            var dobbleTypeSegmentControl = new SegmentControl();
            dobbleTypeSegmentControl.TintColor = Color.Red;
            dobbleTypeSegmentControl.AddSegment("Geheel scherm");
            dobbleTypeSegmentControl.HeightRequest = 30;
            dobbleTypeSegmentControl.AddSegment("Dobbelsteen");
            dobbleTypeSegmentControl.SelectedSegmentChanged += (object sender, int segmentIndex) =>
            {
                dobbleTypeSegmentControl.TintColor = Color.Red;
            };
            dobbleTypeSegmentControl.SelectedSegment = 1;

            SettingsGrid.Children.Add(dobbleTypeSegmentControl, 1, 3);
        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new DicePage());
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
