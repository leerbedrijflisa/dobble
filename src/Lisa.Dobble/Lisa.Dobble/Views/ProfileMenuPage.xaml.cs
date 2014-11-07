using Acr.XamForms.UserDialogs;
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
using Xamarin.Forms.Labs.Mvvm;
using Xamarin.Forms.Labs.Services;
using Xamarin.Forms.Labs.Services.IO;
using Xamarin.Forms.Labs.Services.Media;
using Xamarin.Forms.Labs.Services.SoundService;

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
            _fileManager = DependencyService.Get<IFileManager>();
            SelectDieButton.Clicked += SelectDieButton_Clicked;
            DeleteDieButton.Clicked += DeleteDieButton_Clicked;

            
        }

        private async void InitializeAdditionalComponent()
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
            if(imageSource != null)
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
            _fileManager.CreateDirectory(dice.LastOrDefault().Id.ToString());
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
        private void DeleteDieButton_Clicked(object sender, EventArgs e)
        {
            if (!selectedDie.IsDefault)
            {
                database.DeleteDie(selectedDie.Id);
            }
            dice = database.GetDice();
            ProfileListView.ItemsSource = dice;
        }

        private void RecordSound(object sender, EventArgs e)
        {
            var recordSoundButton = (Button)sender;
            
            var imageCount = 0;
            int.TryParse(recordSoundButton.ClassId, out imageCount);

            _fileStream = _fileManager.OpenFile(String.Format("{0}/{1}.wav", selectedDie.Id, imageCount), FileMode.Create, FileAccess.ReadWrite);

            var microphoneService = DependencyService.Get<IMicrophoneService>();
            _microphone = microphoneService.GetMicrophone();
            _microphone.Start(22050);
            _recorder.StartRecorder(_microphone, _fileStream, 22050);
            recordSoundButton.Text = "Opnemen stoppen";

            selectedDie.Options[imageCount].Sound = String.Format("{0}/{1}.wav", selectedDie.Id, imageCount + 1);
            database.SaveDie(selectedDie);

            recordSoundButton.Clicked -= RecordSound;
            recordSoundButton.Clicked += StopRecording;
            
        }

        private void StopRecording(object sender, EventArgs e)
        {
            var recordSoundButton = (Button)sender;

            _microphone.Stop();
            _recorder.StopRecorder();

            recordSoundButton.Text = "Geluid opnemen";
            recordSoundButton.Clicked -= StopRecording;
            recordSoundButton.Clicked += RecordSound;   
        }

        private WaveRecorder _recorder = new WaveRecorder();
        private IFileManager _fileManager;
        private Stream _fileStream;
        private Stream _tempFileStream;
        private string _fullPath;
        private IAudioStream _microphone;

        private Stream imageSourceStream;
    }
}
