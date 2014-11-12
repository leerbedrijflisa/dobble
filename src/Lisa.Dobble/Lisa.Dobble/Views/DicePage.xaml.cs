using Lisa.Dobble.Models;
using Lisa.Dobble;
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
        public int DobbleDelay;
        private bool IsPopped = false;
        private ILisaSoundService soundService;
        private IPathService pathService;
        
        public DicePage()
        {
            InitializeComponent();
            var device = Resolver.Resolve<IDevice>();
            fileManager = DependencyService.Get<IFileManager>();
            soundService = DependencyService.Get<ILisaSoundService>();
            pathService = DependencyService.Get<IPathService>();
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
            if (!IsPopped && e.Value.Z > 0.890)
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
            SetDieImage(SelectedDie.Options[0].Image);
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
                soundService.Stop();
                if(imageSourceStream != null)
                {
                    imageSourceStream.Dispose();
                }

                soundService = DependencyService.Get<ILisaSoundService>();
                var fullpath = Device.OnPlatform(
                    iOS: "dice.wav",
                    Android: "dice.wav",
                    WinPhone: "dice.wav");
                soundService.PlayAsync(fullpath);

                enabled = false;

                random = new Random();
                NextDie();

                TimeOne.IsVisible = true;
                TimeTwo.IsVisible = true;
                TimeThree.IsVisible = true;
                TimeOne.Opacity = 1;
                TimeTwo.Opacity = 1;
                TimeThree.Opacity = 1;
                int delay = DobbleDelay;
                delay = delay * 1000;
                if (delay >= 250)
                {
                    TimeOne.IsVisible = true;
                    TimeTwo.IsVisible = true;
                    TimeThree.IsVisible = true;
                }
                else
                {
                    TimeOne.IsVisible = false;
                    TimeTwo.IsVisible = false;
                    TimeThree.IsVisible = false;
                }

                Device.StartTimer(new TimeSpan(0, 0, 0, 0, (delay / 3) - 250), () =>
                {
                    TimeOne.FadeTo(0, 250);
                    return false;
                });

                Device.StartTimer(new TimeSpan(0, 0, 0, 0, ((delay / 3) * 2) - 250), () =>
                {
                    TimeTwo.FadeTo(0, 250);
                    return false;
                });

                Device.StartTimer(new TimeSpan(0, 0, 0, 0, delay - 250), () =>
                {
                    TimeThree.FadeTo(0, 250);
                    return false;
                });
                
                Device.StartTimer(new TimeSpan(0, 0, 0, 0, delay), () =>
                    {
                        
                        enabled = true;
                        return false;
                    });
            }
        }

        private void SetDieImage(string image)
        {
            if (image == "notset.png")
            {
                DieView.Source = Device.OnPlatform(
                iOS: ImageSource.FromFile("notset.png"),
                Android: ImageSource.FromFile("Drawable/notset.png"),
                WinPhone: ImageSource.FromFile("notset.png"));
            }
            else if (SelectedDie.IsDefault)
            {
                DieView.Source = Device.OnPlatform(
                    iOS: ImageSource.FromFile("Dice/" + image),
                    Android: ImageSource.FromFile("Drawable/dice/" + image),
                    WinPhone: ImageSource.FromFile("dice/" + image));
            }
            else
            {
                imageSourceStream = fileManager.OpenFile(image, FileMode.Open, FileAccess.Read);
                DieView.Source = ImageSource.FromStream(() => imageSourceStream);
            }
        }

        private void NextDie()
        {
            
            int randomNumber = random.Next(0, SelectedDie.Options.Count());
            var imageName = SelectedDie.Options[randomNumber].Image;
            var soundName = SelectedDie.Options[randomNumber].Sound;

            if (imageName == "notset.png")
            {
                NextDie();
            }
            else
            {
                if (soundName != null)
                {
                    var filePath = pathService.CreateDocumentsPath(soundName);
                    if (SelectedDie.IsDefault)
                    {
                        filePath = "Dice/" + SelectedDie.Options[randomNumber].Sound;
                    }
                    soundService.PlayAsync(filePath);
                }
                SetDieImage(imageName);
            }

        }

        private Random random;
        private IFileManager fileManager;
        private Stream imageSourceStream;

    }
}
