﻿using Lisa.Dobble.Data;
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
        DieDatabase database;
        public SettingsPage()
        {
            database = new DieDatabase();
            var dice = database.GetDice();
            //if(dice.Count() == 0)
            //{
                CreateSampleDice();
            //}
            InitializeComponent();
            InitializeAdditionalComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            StartButton.Clicked +=StartButton_Clicked;
            ChooseProfileButton.Clicked += ChooseProfileButton_Clicked;
        }

        void ChooseProfileButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ProfileMenuPage());
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
            CreateSampleDice();
            DicePage.SelectedDie = database.GetDice().LastOrDefault();
            DicePage.SelectedTouchMode = touchMode;
            Navigation.PushAsync(DicePage);
        }
        private void CreateSampleDice()
        {
            database.RemoveDice();
            var firstDie = new Die();
            firstDie.Name = "Zwart / Wit";
            firstDie.Options = new List<Option>();
            for(var i = 0; i < 6; i++)
            {
                var option = new Option();
                option.Image = String.Format("1/{0}.png", i + 1);
                firstDie.Options.Add(option);
            }

            var secondDie = new Die();
            secondDie.Name = "Geel / Blauw";
            secondDie.Options = new List<Option>();
            for(var i = 0; i < 6; i++)
            {
                var option = new Option();
                option.Image = String.Format("2/{0}.png", i + 1);
                secondDie.Options.Add(option);
            }

            database.InsertDie(firstDie);
            database.InsertDie(secondDie);
        }

        private TouchMode touchMode;
    }
}
