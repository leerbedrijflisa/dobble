﻿using Lisa.Dobble.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Labs;
using Xamarin.Forms.Labs.Services;

namespace Lisa.Dobble
{
    public partial class DicePage
    {
        public Die SelectedDie;
        public TouchMode SelectedTouchMode;
        public bool enabled;
        private bool IsPopped = false;
        public DicePage()
        {
            InitializeComponent();
            var device = Resolver.Resolve<IDevice>();
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
            if (SelectedDie == null)
            {
                
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
                TimeOne.IsVisible = true;
                TimeTwo.IsVisible = true;
                TimeThree.IsVisible = true;
                TimeOne.Opacity = 1;
                TimeTwo.Opacity = 1;
                TimeThree.Opacity = 1;

                Device.StartTimer(new TimeSpan(0, 0, 0, 1, 600), () =>
                {
                    TimeOne.FadeTo(0, 250);
                    return false;
                });

                Device.StartTimer(new TimeSpan(0, 0, 0, 3, 200), () =>
                {
                    TimeTwo.FadeTo(0, 250);
                    return false;
                });

                Device.StartTimer(new TimeSpan(0, 0, 0, 5, 00), () =>
                {
                    TimeThree.FadeTo(0, 250);
                    return false;
                });

                Device.StartTimer(new TimeSpan(0, 0, 5), () =>
                    {
                        
                        enabled = true;
                        return false;
                    });
            }
        }


    }
}
