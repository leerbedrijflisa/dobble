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
        public Die selectedDie;
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
            
            if (selectedDie == null)
            {
                selectedDie = database.GetDice().FirstOrDefault();
            }
            SetDie(selectedDie.Id);
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
                if (dieOptionLayout.Children.OfType<Image>().FirstOrDefault() != null)
                {
                    var imageObject = ((StackLayout)dieOptionLayout).Children.OfType<Image>().FirstOrDefault();
                    imageObject.GestureRecognizers.Add(tapGestureRecognizer);
                }
            }

            fileManager = DependencyService.Get<IFileManager>();
            _soundService = DependencyService.Get<ILisaSoundService>();
            _pathService = DependencyService.Get<IPathService>();
            DieName.Clicked += ChangeDieName;
        }

        private async void ChangeDieName(object sender, object args)
        {
            if(selectedDie.IsDefault)
            {
                DisplayAlert("Fout", "Je kunt de naam van een deze dobbelsteen niet veranderen", "OK");
                return;
            }
            var _userDialogService = DependencyService.Get<IUserDialogService>();
            var r = await _userDialogService.PromptAsync("Vul de naam in van deze dobbelsteen.", "Opslaan");
            if(r.Ok)
            {
                if (r.Text.Length < 1){
                    DisplayAlert("Fout", "Naam van de dobbelsteen moet minimaal 1 karakter lang zijn", "OK");
                }
                else{
                    DieName.Text = r.Text;
                    selectedDie.Name = r.Text;
                    database.SaveDie(selectedDie);
                    RefreshProfileList();
                }
            }
        }

        private void RefreshProfileList()
        {
            dice = database.GetDice();
            ProfileListView.ItemsSource = dice;
        }

        protected override void OnDisappearing()
        {
            if(imageSourceStream != null)
                imageSourceStream.Dispose();
            MessagingCenter.Send<ProfileMenuPage, Die>(this, "SetDie", selectedDie);

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

                var imageFile = fileManager.OpenFile(String.Format("{0}/{1}.png", selectedDie.Id, option), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                imageStream.CopyTo(imageFile);

                selectedDie.Options[option].Image = String.Format("{0}/{1}.png", selectedDie.Id, option);

                database.SaveDie(selectedDie);
                var ok = database.GetDice();
                dice = database.GetDice();

                SetDie(selectedDie.Id);
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
            SetDie(dice.LastOrDefault().Id);
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
            if(selectedDie.IsDefault)
            {
                DeleteDieButton.IsVisible = false;
                foreach (var dieOptionLayout in ProfileGrid.Children.OfType<StackLayout>())
                {
                    var recordSoundButton = ((StackLayout)dieOptionLayout).Children.OfType<Button>().Where(X => X.Text == "Geluid opnemen").FirstOrDefault();
                    if(recordSoundButton != null)
                        recordSoundButton.IsVisible = false;
                    
                    SelectDieButton.IsVisible = false;
                    
                    var interactionButtons = ((StackLayout)dieOptionLayout).Children.OfType<StackLayout>().FirstOrDefault();
                    if (interactionButtons != null)
                    {
                        var recordButton = (Button)interactionButtons.Children.OfType<Button>().LastOrDefault();
                        recordButton.IsEnabled = false;
                    }
                    
                }
            }
            else
            {
                DeleteDieButton.IsVisible = true;
                foreach (var dieOptionLayout in ProfileGrid.Children.OfType<StackLayout>())
                {
                    var recordSoundButton = ((StackLayout)dieOptionLayout).Children.OfType<Button>().Where(X => X.Text == "Geluid opnemen").FirstOrDefault();
                    if(recordSoundButton != null)
                        recordSoundButton.IsVisible = true;

                    SelectDieButton.IsVisible = true;

                    var interactionButtons = ((StackLayout)dieOptionLayout).Children.OfType<StackLayout>().FirstOrDefault();
                    if (interactionButtons != null)
                    {
                        var recordButton = (Button)interactionButtons.Children.OfType<Button>().LastOrDefault();
                        recordButton.IsEnabled = true;
                    }
                }
            }
            DieName.Text = selectedDie.Name;
            SetImages(selectedDie);
        }

        private void SetImages(Die die)
        {
            var count = 0;
            foreach (var dieOptionLayout in ProfileGrid.Children.OfType<StackLayout>())
            {
                var imageObject = ((StackLayout)dieOptionLayout).Children.OfType<Image>().FirstOrDefault();
                if (imageObject == null)
                    return;
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
                    var fullPath = _pathService.CreateDocumentsPath(die.Options[count].Image);
                    dieImage.Source = ImageSource.FromFile(fullPath);
                }
                count++;
            }
        }
        private void DeleteDieButton_Clicked(object sender, EventArgs e)
        {
            if (!selectedDie.IsDefault)
            {
                database.DeleteDie(selectedDie.Id);
                RefreshProfileList();
                SetDie(dice.LastOrDefault().Id);
            }
        }

        private void PlaySound(object sender, EventArgs e)
        {
            var playSoundButton = (Button)sender;

            var dieCount = 0;
            int.TryParse(playSoundButton.ClassId, out dieCount);

            var fullpath = _pathService.CreateDocumentsPath(selectedDie.Options[dieCount].Sound);
            if (selectedDie.IsDefault)
            {
                fullpath = "Dice/" + selectedDie.Options[dieCount].Sound;
            }
            _soundService.PlayAsync(fullpath);
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
            recordSoundButton.Image = "stop.png";

            selectedDie.Options[imageCount].Sound = String.Format("{0}/{1}.wav", selectedDie.Id, imageCount);
            database.SaveDie(selectedDie);

            recordSoundButton.Clicked -= RecordSound;
            recordSoundButton.Clicked += StopRecording;
            
        }

        private void StopRecording(object sender, EventArgs e)
        {
            var recordSoundButton = (Button)sender;

            _microphone.Stop();
            _recorder.StopRecorder();

            recordSoundButton.Image = "record.png";
            recordSoundButton.Clicked -= StopRecording;
            recordSoundButton.Clicked += RecordSound;   
        }

        private WaveRecorder _recorder = new WaveRecorder();
        private IUserDialogService _userDialogService;
        private IFileManager _fileManager;
        private Stream _fileStream;
        private string _fullPath;
        private IAudioStream _microphone;
        private IPathService _pathService;
        private ILisaSoundService _soundService;
        private Stream imageSourceStream;
    }
}
