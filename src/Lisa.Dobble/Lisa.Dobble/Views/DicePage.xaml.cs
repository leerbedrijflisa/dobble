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
using Xamarin.Forms.Labs.Mvvm;

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
            fileManager = DependencyService.Get<IFileManager>();
            soundService = DependencyService.Get<ILisaSoundService>();
            pathService = DependencyService.Get<IPathService>();
            NavigationPage.SetHasNavigationBar(this, false);
            TimeOne.IsVisible = false;
            TimeTwo.IsVisible = false;
            TimeThree.IsVisible = false;
            enabled = true;

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
            _timer.Tick -= OnTick;
            _timer.Stop();
            _app.Resumed -= PushSettingsPage;
            base.OnDisappearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void InitializeAdditionalComponent()
        {
            firstDie = true;
            NextDie();
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.TappedCallback += (s, e) =>
            {
                RollDice();
            };

            if(SelectedTouchMode == TouchMode.Die)
            {
                DieLayout.GestureRecognizers.Add(tapGestureRecognizer);
            }
            else
            {
                MainGrid.GestureRecognizers.Add(tapGestureRecognizer);
            }
            var device = Resolver.Resolve<IDevice>();
            device.Accelerometer.Interval = AccelerometerInterval.Normal;
            device.Accelerometer.ReadingAvailable += Accelerometer_ReadingAvailable;

            _timer = new Timer();
            _timer.Tick += OnTick;

            _app = Resolver.Resolve<IXFormsApp>();
            _app.Resumed += PushSettingsPage;
        }

        private void PushSettingsPage(object sender, EventArgs e)
        {
            _app.Resumed -= PushSettingsPage;
            Navigation.PopAsync();
        }

        void OnTick(object sender, EventArgs e)
        {
            StartSnoozeAnimation();
        }

        private async void RollDice()
        {

            if (enabled && !isAnimating)
            {   
                _timer.Stop();
                needsAnimation = false;
                Device.StartTimer(new TimeSpan(0, 0, 0, 9), () =>
                {
                    needsAnimation = true;
                    return false;
                });
                soundService.Stop();
                if(imageSourceStream != null)
                {
                    imageSourceStream.Dispose();
                }
                Instructions.IsVisible = false;
                soundService = DependencyService.Get<ILisaSoundService>();
                var fullpath = Device.OnPlatform(
                    iOS: "dice.wav",
                    Android: "dice.wav",
                    WinPhone: "dice.wav");
                soundService.PlayAsync(fullpath);

                enabled = false;
                int delay = DobbleDelay;
                delay = delay * 1000;
                if (!(delay <= 250))
                {
                    //TimeOne.IsVisible = false;
                    //TimeTwo.IsVisible = false;
                    //TimeThree.IsVisible = false;
                }
                await StartRollOutAnimation();
                
               

 

               
            }
        }

        private void SetDieImage(string image)
        {
            if (image == "white.png")
            {
                DieView.Source = Device.OnPlatform(
                iOS: ImageSource.FromFile("Dice/white.png"),
                Android: ImageSource.FromFile("Drawable/dice/white.png"),
                WinPhone: ImageSource.FromFile("dice/white.png"));
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
                var fullPath = pathService.CreateDocumentsPath(image);
                DieView.Source = ImageSource.FromFile(fullPath);
            }
            StartRollInAnimation();
 

        }

        private void NextDie()
        {
            if(random == null)
            {
                random = new Random();
            }
            
            int randomNumber = random.Next(0, SelectedDie.Options.Count());
            var imageName = SelectedDie.Options[randomNumber].Image;
            var soundName = SelectedDie.Options[randomNumber].Sound;

            if (imageName == "notset.png")
            {
                NextDie();
            }
            else
            {
                if (soundName != null && !firstDie  )
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

        private async void StartSnoozeAnimation()
        {
            var xPosition = DieLayout.X;
            var yPosition = DieLayout.Y;
            if (!isAnimating)
            {
                var filePath = Device.OnPlatform(
                    iOS: "snoozesound.mp3",
                    Android: "snoozesound.mp3",
                    WinPhone: "snoozesound.mp3");
                soundService.PlayAsync(filePath);
                try
                {
                    for (var i = 0; i < 5; i++)
                    {
                        isAnimating = true;

                        Rectangle rec = new Rectangle(xPosition + 10, yPosition, 367, 367);

                        Rectangle rec2 = new Rectangle(rec.X - 20, yPosition, 367, 367);

                        Rectangle rec3 = new Rectangle(rec2.X + 10, yPosition, 367, 367);
                        
                        await DieLayout.LayoutTo(rec, 35);
                        await DieLayout.LayoutTo(rec2, 35);
                        DieLayout.FadeTo(0.4, 200);
                        await DieLayout.LayoutTo(rec3, 35);
                    }
                }catch(Exception e)
                {

                }
                isAnimating = false;
            }
        }

        private async Task StartRollOutAnimation()
        {
            enabled = false;
            isAnimating = true;
            var xPosition = DieLayout.X;
            var yPosition = DieLayout.Y;
            await DieLayout.FadeTo(1, 200);
            Rectangle rec = new Rectangle(MainGrid.Width, yPosition, 367, 367);
            DieLayout.LayoutTo(rec, 1000, Easing.Linear);
            await DieLayout.RelRotateTo(220, 750);
            isAnimating = false;
            NextDie();
            
        }

        private async void StartRollInAnimation()
        {
            if (!firstDie)
            {
                if (DobbleDelay > 0)
                {
                    TimeOne.IsVisible = true;
                    TimeTwo.IsVisible = true;
                    TimeThree.IsVisible = true;
                    TimeOne.Opacity = 0;
                    TimeTwo.Opacity = 0;
                    TimeThree.Opacity = 0;
                    TimeOne.FadeTo(1, 250);
                    TimeTwo.FadeTo(1, 250);
                    TimeThree.FadeTo(1, 250);
                }
                else
                {
                    TimeOne.IsVisible = false;
                    TimeTwo.IsVisible = false;
                    TimeThree.IsVisible = false;
                }
                
            }

           
            if(DieMask.Source == null)
            {
                DieMask.Source = Device.OnPlatform(
                    iOS: ImageSource.FromFile("dobblemask.png"),
                    Android: ImageSource.FromFile("Drawable/dobblemask.png"),
                    WinPhone: ImageSource.FromFile("dobblemask.png"));
            }
            isAnimating = true;
            var xPosition = DieLayout.X;
            var yPosition = DieLayout.Y;
            Rectangle rec = new Rectangle(-600, yPosition, 367, 367);
            Rectangle rec2 = new Rectangle(xPosition, yPosition, 367, 367);
            DieLayout.Layout(rec);
            DieLayout.Rotation = 160;
            await DieLayout.LayoutTo(rec, 0);
            DieLayout.LayoutTo(rec2, 750, Easing.Linear);
            await DieLayout.RelRotateTo(200, 750);
            DieLayout.VerticalOptions = LayoutOptions.Center;
            isAnimating = false;
            Rectangle rec3 = new Rectangle(xPosition, (DieGrid.Height / 2) - (367 / 2) - 13, 367, 367);
            DieLayout.Layout(rec3);
            if (!firstDie)
            {
                int delay = DobbleDelay;
                delay = delay * 1000;
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
                if (delay < 3)
                {
                    Device.StartTimer(new TimeSpan(0, 0, 0, 1), () =>
                    {
                        enabled = true;

                        return false;
                    });
                }
                else
                {
                    Device.StartTimer(new TimeSpan(0, 0, 0, 0, delay), () =>
                    {
                        enabled = true;
                        DieLayout.FadeTo(0.4, 200);
                        return false;
                    });
                }
                _timer.Start(DobbleDelay * 1000 + 10000);
            }
            else
            {
                firstDie = false;
            }
            
        }

        private Random random;
        private IFileManager fileManager;
        private Timer _timer;
        private bool needsAnimation;
        private IXFormsApp _app;
        private bool isAnimating;
        private bool firstDie;
        private Stream imageSourceStream;

    }
}
