using Lisa.Dobble.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Labs;
using Xamarin.Forms.Labs.Services;
using Xamarin.Forms.Labs.Services.IO;
using Xamarin.Forms.Labs.Services.SoundService;

namespace Lisa.Dobble
{
    public partial class DicePage
    {
        public Die SelectedDie;
        public TouchMode SelectedTouchMode;
        public bool enabled;
        public string DobbleDelay;
        private bool IsPopped = false;
        
        public DicePage()
        {
            InitializeComponent();
            var device = Resolver.Resolve<IDevice>();
            fileManager = DependencyService.Get<IFileManager>();
            NavigationPage.SetHasNavigationBar(this, false);
            TimeOne.IsVisible = false;
            TimeTwo.IsVisible = false;
            TimeThree.IsVisible = false;
            enabled = true;
            device.Accelerometer.Interval = AccelerometerInterval.Normal;
            device.Accelerometer.ReadingAvailable += Accelerometer_ReadingAvailable;
        }

        void Accelerometer_ReadingAvailable(object sender, EventArgs<Xamarin.Forms.Labs.Helpers.Vector3> e)
        {
            if(e.Value.Y < 0.3 && !IsPopped && e.Value.Z < 0)
            {
                Navigation.PopAsync();
                IsPopped = true;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitializeAdditionalComponent();
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

        private void RollDice()
        {
            if (enabled)
            {
                if(imageSourceStream != null)
                {
                    imageSourceStream.Dispose();
                }

                ISoundService soundService = DependencyService.Get<ISoundService>();
                var fullpath = Device.OnPlatform(
                    iOS: "dice.wav",
                    Android: "dice.wav",
                    WinPhone: "dice.wav");
                soundService.PlayAsync(fullpath);

                enabled = false;
                var random = new Random();
                int randomNumber = random.Next(0, SelectedDie.Options.Count());
                var imageName = SelectedDie.Options[randomNumber].Image;
                if (imageName == "notset.png")
                {
                    DieView.Source = Device.OnPlatform(
                    iOS: ImageSource.FromFile("notset.png"),
                    Android: ImageSource.FromFile("Drawable/notset.png"),
                    WinPhone: ImageSource.FromFile("notset.png"));
                }
                else if (SelectedDie.IsDefault)
                {
                    DieView.Source = Device.OnPlatform(
                        iOS: ImageSource.FromFile("Dice/" + imageName),
                        Android: ImageSource.FromFile("Drawable/dice/" + imageName),
                        WinPhone: ImageSource.FromFile("dice/" + imageName));
                }
                else
                {
                    imageSourceStream = fileManager.OpenFile(imageName, FileMode.Open, FileAccess.Read);
                    DieView.Source = ImageSource.FromStream(() => imageSourceStream);
                }

                TimeOne.IsVisible = true;
                TimeTwo.IsVisible = true;
                TimeThree.IsVisible = true;
                TimeOne.Opacity = 1;
                TimeTwo.Opacity = 1;
                TimeThree.Opacity = 1;
                int Delay = int.Parse(DobbleDelay);

                Device.StartTimer(new TimeSpan(0, 0, 0, Delay/3, 600), () =>
                {
                    TimeOne.FadeTo(0, 250);
                    return false;
                });

                Device.StartTimer(new TimeSpan(0, 0, 0, (Delay/3)*2, 200), () =>
                {
                    TimeTwo.FadeTo(0, 250);
                    return false;
                });

                Device.StartTimer(new TimeSpan(0, 0, 0, Delay, 00), () =>
                {
                    TimeThree.FadeTo(0, 250);
                    return false;
                });
                
                Device.StartTimer(new TimeSpan(0, 0, Delay), () =>
                    {
                        
                        enabled = true;
                        return false;
                    });
            }
        }

        private IFileManager fileManager;
        private Stream imageSourceStream;

    }
}
