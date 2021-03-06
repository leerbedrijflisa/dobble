﻿using Lisa.Dobble.Models;
using Lisa.Dobble;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Platform.Services.IO;
using XLabs;
using XLabs.Platform.Mvvm;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;

namespace Lisa.Dobble
{
    public partial class DicePage
    {
        public Die SelectedDie;
        public TouchMode SelectedTouchMode;
        public bool enabled;
        public int DobbleDelay;
        private bool IsPopped = false;
        private ISoundService _soundService;
        private IPathService _pathService;
        
        public DicePage()
        {
            InitializeComponent();
			InitializeServices();
            
            NavigationPage.SetHasNavigationBar(this, false);
            TimeOne.IsVisible = false;
            TimeTwo.IsVisible = false;
            TimeThree.IsVisible = false;
            enabled = true;

        }

		private void InitializeServices()
		{
			_soundService = Resolver.Resolve<ISoundService>();
			_pathService = Resolver.Resolve<IPathService>();
		}

        void Accelerometer_ReadingAvailable(object sender, EventArgs<Vector3> e)
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
            var device = Resolver.Resolve<IDevice>();
            device.Accelerometer.ReadingAvailable -= Accelerometer_ReadingAvailable;
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
            if (SelectedTouchMode == TouchMode.Die){
                DieMask.OnSwipe += DieLayout_OnSwipe;
            }
            else
            {
                MainGrid.OnSwipe += MainGrid_OnSwipe;
            }
            _timer = new Timer();
            _timer.Tick += OnTick;

            _app = Resolver.Resolve<IXFormsApp>();
            _app.Resumed += PushSettingsPage;
        }

        void DieLayout_OnSwipe(object sender, EventArgs e)
        {
            RollDice();
        }

        void MainGrid_OnSwipe(object sender, EventArgs e)
        {
            RollDice();
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
                Device.StartTimer(new TimeSpan(0, 0, 0, 9), () =>
                {
                    return false;
                });
                _soundService.Stop();
                if(imageSourceStream != null)
                {
                    imageSourceStream.Dispose();
                }
                Instructions.IsVisible = false;
                var fullpath = Device.OnPlatform(
                    iOS: "dice.wav",
                    Android: "dice.wav",
                    WinPhone: "dice.wav");
                _soundService.PlayAsync(fullpath);

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
                var fullPath = _pathService.CreateDocumentsPath(image);
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

            _randomNumber = random.Next(0, SelectedDie.Options.Count());
            var imageName = SelectedDie.Options[_randomNumber].Image;
            var soundName = SelectedDie.Options[_randomNumber].Sound;

            if (imageName == "notset.png")
            {
                NextDie();
            }
            else
            {   
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
                _soundService.PlayAsync(filePath);
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
                }
				catch (Exception)
                {
					// What is this catch doing here? Will things go wrong if we
					// remove it?
					// TODO: Find out.
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

            var soundName = SelectedDie.Options[_randomNumber].Sound;
            if (soundName != null && !firstDie)
            {
                var filePath = _pathService.CreateDocumentsPath(soundName);
                if (SelectedDie.IsDefault)
                {
                    filePath = "Dice/" + soundName;
                }
                _soundService.PlayAsync(filePath);
            }

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
        private int _randomNumber;
        private Timer _timer;
        private IXFormsApp _app;
        private bool isAnimating;
        private bool firstDie;
        private Stream imageSourceStream;

    }
}
