using Lisa.Dobble.Models;
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
                switch(segmentIndex)
                {
                    case 0:
                        touchMode = TouchMode.Fullscreen;
                        break;
                    case 1:
                        touchMode = TouchMode.Die;
                        break;
                }
            };
            dobbleTypeSegmentControl.SelectedSegment = 1;

            SettingsGrid.Children.Add(dobbleTypeSegmentControl, 1, 3);
        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {
            var DicePage = new DicePage();
            DicePage.SelectedDie = CreateSampleDie();
            DicePage.SelectedTouchMode = touchMode;
            Navigation.PushAsync(DicePage);
        }
        private Die CreateSampleDie()
        {
            var generatedDie = new Die();
            generatedDie.Options = new List<Option>();

            for (var i = 0; i < 6; i++)
            {
                var option = new Option();

                if (i == 0)
                {
                    option.Image = "dice.png";
                }
                else
                {
                    option.Image = String.Format("dice{0}.png", i + 1);
                }

                generatedDie.Options.Add(option);
            }

            generatedDie.Name = "Sample";

            return generatedDie;
        }

        private TouchMode touchMode;
    }
}
