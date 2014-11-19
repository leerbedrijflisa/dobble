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
        private DieDatabase database;
        public Die SelectedDie;
        public SettingsPage()
        {
            database = new DieDatabase();
            var dice = database.GetDice();
            if (dice.Count() == 0)
            {
                CreateDefaultDice();
            }
            InitializeComponent();
            InitializeAdditionalComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            StartButton.Clicked += StartButton_Clicked;
            ChooseProfileButton.Clicked += ChooseProfileButton_Clicked;

            DobbleDelay.Keyboard = Keyboard.Numeric;

            MessagingCenter.Subscribe<ProfileMenuPage, Die>(this, "SetDie", (sender, args) =>
            {
                SelectedDie = args;
            });
        }

        void ChooseProfileButton_Clicked(object sender, EventArgs e)
        {
            var profileMenuPage = new ProfileMenuPage();
            profileMenuPage.selectedDie = SelectedDie;
            Navigation.PushAsync(profileMenuPage);

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (SelectedDie != null)
            {
                DieNameLabel.Text = SelectedDie.Name;
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
            dobbleTypeSegmentControl.SelectedSegment = 0;
            SelectedDie = database.GetDice().FirstOrDefault();
            SettingsGrid.Children.Add(dobbleTypeSegmentControl, 1, 3);
        }
        
        private void StartButton_Clicked(object sender, EventArgs e)
        {
            int dobbleDelay;
            if(!int.TryParse(DobbleDelay.Text, out dobbleDelay))
            {
                DisplayAlert("Fout", "Je kunt alleen maar cijfers invullen bij de wachttijd.", "OK");
                return;
            }
            var DicePage = new DicePage();
            if(SelectedDie == null)
            {
                SelectedDie = database.GetDice().FirstOrDefault();
            }
            DicePage.DobbleDelay = dobbleDelay;
            DicePage.SelectedDie = SelectedDie;
            DicePage.SelectedTouchMode = touchMode;
            Navigation.PushAsync(DicePage);
        }
        private void CreateDefaultDice()
        {
            for (var dice = 1; dice <= 5; dice++)
            {
                var die = new Die();
                die.Name = "Default";
                die.IsDefault = true;
                die.Options = new List<Option>();
                for (var i = 0; i < 6; i++)
                {
                    var option = new Option();
                    option.Image = String.Format(dice.ToString() + "/{0}.png", i + 1);
                    option.Sound = String.Format(dice.ToString() + "/{0}.wav", i + 1);
                    die.Options.Add(option);
                }
                switch (dice)
                {
                    case 1:
                        die.Name = "Stippen (zwart/wit)";
                        break;
                    case 2:
                        die.Name = "Stippen (blauw/geel)";
                        break;
                    case 3:
                        die.Name = "Instrumenten";
                        break;
                    case 4:
                        die.Name = "Kleuren";
                        break;
                    case 5:
                        die.Name = "Vormen (zwart/wit)";
                        break;
                }
                database.InsertDie(die);
            }
        }

        private TouchMode touchMode;
    }
}
