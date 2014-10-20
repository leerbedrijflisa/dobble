using Lisa.Dobble.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lisa.Dobble
{
    public partial class DicePage
    {
        public Die SelectedDie;
        public TouchMode SelectedTouchMode;
        public bool enabled;
        public DicePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            enabled = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitializeAdditionalComponent();
            if (SelectedDie == null)
            {
                CreateSampleDice();
            }
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void InitializeAdditionalComponent()
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.TappedCallback += (s, e) =>
            {
                RollDice();
            };

            if(SelectedTouchMode == TouchMode.Die)
            {
                DieView.GestureRecognizers.Add(tapGestureRecognizer);
            }
            else
            {
                MainGrid.GestureRecognizers.Add(tapGestureRecognizer);
            }
        }

        private void CreateSampleDice()
        {
            SelectedDie = new Die();
            //SelectedDie.Options = new List<Option>();

            for(var i = 0; i < 6; i++)
            {
                var option = new Option();

                if(i == 0)
                {
                    option.Image = "dice.png";
                }else{
                    option.Image = String.Format("dice{0}.png", i + 1);
                }

                //SelectedDie.Options.Add(option);
            }

            SelectedDie.Name = "Sample";
        }

        private void RollDice()
        {
            if (enabled)
            {
                enabled = false;
                var random = new Random();
                int randomNumber = random.Next(0, SelectedDie.Options.Count());
                var imageName = SelectedDie.Options[randomNumber].Image;
                DieView.Source = Device.OnPlatform(
                    iOS: ImageSource.FromFile("Dice/" + imageName),
                    Android: ImageSource.FromFile("Drawable/" + imageName),
                    WinPhone: ImageSource.FromFile(imageName));

                Device.StartTimer(new TimeSpan(0, 0, 5), () =>
                    {
                        enabled = true;
                        return false;
                    });
            }
        }


    }
}
