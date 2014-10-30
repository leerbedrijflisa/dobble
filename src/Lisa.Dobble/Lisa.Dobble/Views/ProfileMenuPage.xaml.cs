using Lisa.Dobble.Data;
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
using Xamarin.Forms.Labs.Services.Media;

namespace Lisa.Dobble
{
    public partial class ProfileMenuPage
    {
        DieDatabase database;
        List<Die> dice;
        Die selectedDie;
        IMediaPicker mediaPicker;
        ImageSource imageSource;
        IFileManager fileManager;

        public ProfileMenuPage()
        {
            InitializeComponent();
            InitializeAdditionalComponent();
            database = new DieDatabase();
            dice = database.GetDice();
            ProfileListView.ItemTapped += dieCell_Tapped;
            ProfileListView.ItemsSource = dice;
            ProfileListView.SelectedItem = 0;
            selectedDie = database.GetDice().LastOrDefault();

            SelectDieButton.Clicked += SelectDieButton_Clicked;
        }

        private void InitializeAdditionalComponent()
        {
            ToolbarItems.Add(new ToolbarItem("Add", "plus.png", async () =>
            {
                CreateNewDie();
            }));

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.TappedCallback += (s, e) =>
            {
                if (!selectedDie.IsDefault)
                {
                    var imageCount = 0;
                    int.TryParse(s.ClassId, out imageCount);
                    SelectPicture(imageCount - 1);
                }
            };

            foreach (var dieOptionLayout in ProfileGrid.Children.OfType<StackLayout>())
            {
                var imageObject = ((StackLayout)dieOptionLayout).Children.OfType<Image>().FirstOrDefault();
                imageObject.GestureRecognizers.Add(tapGestureRecognizer);
            }

            fileManager = DependencyService.Get<IFileManager>();
        }

        protected override void OnDisappearing()
        {
            imageSourceStream.Dispose();
            base.OnDisappearing();
        }

        private async Task SelectPicture(int option = 1)
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

                var imageStream = mediaFile.Source;
                fileManager.CreateDirectory(selectedDie.Id.ToString());

                var imageFile = fileManager.OpenFile(String.Format("{0}/{1}.png", selectedDie.Id, option), FileMode.Create, FileAccess.ReadWrite);
                imageStream.CopyTo(imageFile);

                selectedDie.Options[option].Image = String.Format("{0}/{1}.png", selectedDie.Id, option);

                database.SaveDie(selectedDie);
                var ok = database.GetDice();
                dice = database.GetDice();
                
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
                }
                else if(die.IsDefault)
                {
                    dieImage.Source = Device.OnPlatform(
                        iOS: ImageSource.FromFile("Dice/" + die.Options[count].Image),
                        Android: ImageSource.FromFile("Drawable/dice/" + die.Options[count].Image),
                        WinPhone: ImageSource.FromFile("dice/" + die.Options[count].Image));
                }else
                {
                    imageSourceStream = fileManager.OpenFile(die.Options[count].Image, FileMode.Open, FileAccess.Read);
                    dieImage.Source = ImageSource.FromStream(() => imageSourceStream);
                    imageSourceStream.Flush();
                }
                count++;
            }
        }

        private Stream imageSourceStream;
    }
}
