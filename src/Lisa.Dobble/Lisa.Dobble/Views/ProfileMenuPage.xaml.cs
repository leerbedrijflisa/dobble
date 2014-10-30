using Lisa.Dobble.Data;
using Lisa.Dobble.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Labs;
using Xamarin.Forms.Labs.Services;
using Xamarin.Forms.Labs.Services.Media;

namespace Lisa.Dobble
{
    public partial class ProfileMenuPage
    {
        DieDatabase database;
        IEnumerable<Die> dice;
        Die selectedDie;
        IMediaPicker mediaPicker;
        ImageSource imageSource;

        public ProfileMenuPage()
        {
            InitializeComponent();
            database = new DieDatabase();
            dice = database.GetDice();
            ProfileListView.ItemTapped += dieCell_Tapped;
            ProfileListView.ItemsSource = dice;

            ToolbarItems.Add(new ToolbarItem("Add", "plus.png", () =>
            {
                CreateNewDie();
            }));

            SelectDieButton.Clicked += SelectDieButton_Clicked;
        }
        private async Task SelectPicture()
        {
            Setup();

            imageSource = null;
            try
            {
                var mediaFile = await this.mediaPicker.SelectPhotoAsync(new CameraMediaStorageOptions
                {
                    DefaultCamera = CameraDevice.Front,
                    MaxPixelDimension = 400
                });
                imageSource = ImageSource.FromStream(() => mediaFile.Source);
            }
            catch (System.Exception ex)
            {
                //this.Status = ex.Message;
            }
        }

        private void Setup()
        {
            if (mediaPicker != null)
            {
                return;
            }

            var device = Resolver.Resolve<IDevice>();

            mediaPicker = DependencyService.Get<IMediaPicker>();
            ////RM: hack for working on windows phone? 
            if (mediaPicker == null)
            {
                mediaPicker = device.MediaPicker;
            }
        }

        private void CreateNewDie()
        {
            var firstDie = new Die();
            firstDie.Name = String.Format("Dobbelsteen ({0})", database.GetDice().Count());
            firstDie.IsDefault = false;
            firstDie.Options = new List<Option>();
            for (var i = 0; i < 6; i++)
            {
                var option = new Option();
                option.Image = "notset.png";
                firstDie.Options.Add(option);
            }

            database.InsertDie(firstDie);
            dice = database.GetDice();

            ProfileListView.ItemsSource = dice;
        }

        void SelectDieButton_Clicked(object sender, EventArgs e)
        {
            var settingsPage = new SettingsPage();
            settingsPage.SelectedDie = selectedDie;
            Navigation.PushAsync(settingsPage);
        }

        private void dieCell_Tapped(object sender, ItemTappedEventArgs args)
        {
            var tappedDieCell = args.Item as Die;
            SetDie(tappedDieCell.Id);
        }

        private void SetDie(int dieId)
        {
            selectedDie = dice.Where(x => x.Id == dieId).FirstOrDefault();
            SetImages(selectedDie);
        }

        private void SetImages(Die die)
        {
            var count = 0;
            foreach (var dieOptionLayout in ProfileGrid.Children.OfType<StackLayout>())
            {
                var imageObject = ((StackLayout)dieOptionLayout).Children.OfType<Image>().FirstOrDefault();
                var dieImage = (Image)imageObject;
                if (die.Options[count].Image == "notset.png")
                {
                    dieImage.Source = Device.OnPlatform(
                    iOS: ImageSource.FromFile("notset.png"),
                    Android: ImageSource.FromFile("Drawable/notset.png"),
                    WinPhone: ImageSource.FromFile("notset.png"));
                }else{
                    dieImage.Source = Device.OnPlatform(
                        iOS: ImageSource.FromFile("Dice/" + die.Options[count].Image),
                        Android: ImageSource.FromFile("Drawable/dice/" + die.Options[count].Image),
                        WinPhone: ImageSource.FromFile("dice/" + die.Options[count].Image));
                }
                count++;
            }
        }
    }
}
