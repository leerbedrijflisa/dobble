using Lisa.Dobble.Data;
using Lisa.Dobble.Interfaces;
using Lisa.Dobble.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Platform.Services.Media;
using XLabs.Platform.Services.IO;
using XLabs.Platform.Mvvm;
using XLabs.Ioc;
using XLabs.Platform.Device;
using Acr.UserDialogs;
using System.Diagnostics;

namespace Lisa.Dobble
{
    public partial class ProfileMenuPage
    {
        DieDatabase database;
        List<Die> dice;
        public Die selectedDie;
        IMediaPicker mediaPicker;

        public ProfileMenuPage()
        {
            InitializeComponent();
            InitializeAdditionalComponent();
			InitializeServices();

            database = new DieDatabase();
            dice = database.GetDice();
            ProfileListView.ItemTapped += dieCell_Tapped;
            ProfileListView.ItemsSource = dice;
            ProfileListView.SelectedItem = 0;

			_soundService.SoundFileFinished += (object sender, EventArgs e) => _soundService.Stop();
            SelectDieButton.Clicked += SelectDieButton_Clicked;
            DeleteDieButton.Clicked += DeleteDieButton_Clicked;
        }

        private void InitializeAdditionalComponent()
        {
			ToolbarItems.Add(new ToolbarItem("Add", "plus.png", () =>
            {
                CreateNewDie();
            }));

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.TappedCallback += async (s, e) =>
            {
                if (!selectedDie.IsDefault)
                {
                    var imageCount = 0;
                    int.TryParse(s.ClassId, out imageCount);
                    await SelectPicture(imageCount - 1);
                }
            };

            foreach (var dieOptionLayout in ProfileGrid.Children.OfType<StackLayout>())
            {
                if (dieOptionLayout.Children.OfType<AbsoluteLayout>().FirstOrDefault() != null)
                {
                    var layoutObject = ((StackLayout)dieOptionLayout).Children.OfType<AbsoluteLayout>().FirstOrDefault();
                    layoutObject.GestureRecognizers.Add(tapGestureRecognizer);
                }
            }
				
            DieName.Clicked += ChangeDieName;
            DieNameIcon.Clicked += ChangeDieName;

            _app = Resolver.Resolve<IXFormsApp>();
            _app.Resumed += PushSettingsPage;
        }

		private void InitializeServices()
		{
			_app = Resolver.Resolve<IXFormsApp>();
			_fileManager = Resolver.Resolve<IFileManager>();
			_pathService = Resolver.Resolve<IPathService>();
			_soundService = Resolver.Resolve<ISoundService>();
			_imageService = Resolver.Resolve<IImageResizerService>();
		}

        private void PushSettingsPage(object sender, EventArgs e)
        {
            _app.Resumed -= PushSettingsPage;
            Navigation.PopAsync();
        }

        private async void ChangeDieName(object sender, object args)
        {
            if(selectedDie.IsDefault)
            {
                await DisplayAlert("Fout", "Je kunt de naam van deze dobbelsteen niet veranderen", "OK");
                return;
            }
			var userDialogs = Resolver.Resolve<IUserDialogs>();
            var r = await userDialogs.PromptAsync("Vul de naam in van deze dobbelsteen.", "Opslaan");
            if(r.Ok)
            {
                if (r.Text.Length < 1){
                    await DisplayAlert("Fout", "Naam van de dobbelsteen moet minimaal 1 karakter lang zijn", "OK");
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

        protected override void OnAppearing()
        {
            if (selectedDie == null)
            {
                selectedDie = database.GetDice().FirstOrDefault();
            }
            if (!_isCameraOpen)
            {
                _previousSelectedDie = selectedDie;
                SetDie(selectedDie.Id);
            }

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            StopRecording();

            if (!_isCameraOpen)
            {
                _app.Resumed -= PushSettingsPage;
                if (selectedDie.Options.All(option => option.Image == "notset.png") || selectedDie.Id == 2 || selectedDie.Id == 4 || selectedDie.Id == 5)
                {
                    if (!selectedDie.IsDefault)
                    {
                        database.DeleteDie(selectedDie.Id);
                    }
                    selectedDie = _previousSelectedDie;
                }
                else
                {
                    if (imageSourceStream != null)
                        imageSourceStream.Dispose();
                    MessagingCenter.Send<ProfileMenuPage, Die>(this, "SetDie", selectedDie);

                    base.OnDisappearing();
                }

                var dice = database.GetDice().Where(die => die.Options.All(option => option.Image == "notset.png"));
                foreach (Die die in dice)
                {
                    database.DeleteDie(die.Id);
                }
            }
        }

        private async Task SelectPicture(int option = 1)
        {
            Setup();
            try
            {
                var action = await DisplayActionSheet("Foto selecteren", "Annuleren", null, "Camera", "Galerij");
                MediaFile mediaFile = null;
                _isCameraOpen = true;
                switch (action)
                {
                    case "Annuleren":
                        return;
                    case "Camera":
                        mediaFile = await this.mediaPicker.TakePhotoAsync(new CameraMediaStorageOptions
                        {
                            SaveMediaOnCapture = true,
                            DefaultCamera = CameraDevice.Rear,
                            MaxPixelDimension = 400,
                        });
                        break;
                    case "Galerij":
                        mediaFile = await this.mediaPicker.SelectPhotoAsync(new CameraMediaStorageOptions());
                        break;
                    default:
                        break;
                }

                var imageStream = mediaFile.Source;
                byte[] imageData;
                using(MemoryStream ms = new MemoryStream())
                {
                    imageStream.CopyTo(ms);
                    imageData = ms.ToArray();
                }

                byte[] resizedImage = _imageService.ResizeImage(imageData, 367, 367);

                _fileManager.CreateDirectory(selectedDie.Id.ToString());

                var imageFile = _fileManager.OpenFile(String.Format("{0}/{1}.png", selectedDie.Id, option), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                var tempStream = new MemoryStream(resizedImage);
                tempStream.CopyTo(imageFile);
                _isCameraOpen = false;

                selectedDie.Options[option].Image = String.Format("{0}/{1}.png", selectedDie.Id, option);

                database.SaveDie(selectedDie);
                dice = database.GetDice();

                SetDie(selectedDie.Id);
            }
            catch (System.Exception ex)
            {
				// What is this catch doing here? Will things break if we remove it?
				// TODO: Find out.
            }
        }

        private void FadeOutDieImages(Layout<View> layoutView)
        {
            foreach(var element in layoutView.Children)
            {
                if(element.GetType() == typeof(StackLayout))
                {
                    var dieimage = ((StackLayout)element).Children.FirstOrDefault();
                    if (dieimage != null)
                    {
                        if (dieimage.GetType() == typeof(AbsoluteLayout))
                            dieimage.FadeTo(0.4, 100);
                    }
                }
            }
        }
        private void FadeInDieImages(Layout<View> layoutView)
        {
            foreach (var element in layoutView.Children)
            {
                if (element.GetType() == typeof(StackLayout))
                {
                    var dieimage = ((StackLayout)element).Children.FirstOrDefault();
                    if (dieimage != null)
                    {
						if (dieimage.GetType() == typeof(AbsoluteLayout))
						{
							dieimage.FadeTo(1, 250);
						}
                    }
                }
            }
        }
			
        private void DisableInteraction(Layout<View> layoutView)
        {
            foreach (var element in layoutView.Children)
            {
				if(element is Layout<View>)
                {
                    DisableInteraction((Layout<View>)element);
                }else{
                    ((View)element).IsEnabled = false;
                }
            }
        }

        private void EnableInteraction(Layout<View> layoutView)
        {
            foreach (var element in layoutView.Children)
            {
                if (element is Layout<View>)
                {
                    EnableInteraction((Layout<View>)element);
                }
                else
                {
                    ((View)element).IsEnabled = true;
                }
            }
        }

        private void Setup()
        {
            if (mediaPicker != null)
            {
                return;
            }

			mediaPicker = Resolver.Resolve<IMediaPicker>();

			////RM: hack for working on windows phone? 
            if (mediaPicker == null)
            {
				var device = Resolver.Resolve<IDevice>();
                mediaPicker = device.MediaPicker;
            }
        }

        private void CreateNewDie()
        {
#if FREE
            if(dice.Where(x => x.IsDefault == false).Count() >= 1)
            {
                DisplayAlert("Fout", "Je zult de app moeten kopen als je meer dan 1 profiel wilt aanmaken.", "OK");
                return;
            }
#endif
            StopRecording();
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
            StopRecording();
            if (selectedDie.Options.All(option => option.Image == "notset.png"))
            {
                DisplayAlert("Fout", "Je moet minimaal één afbeelding toevoegen aan je dobbelsteen.", "OK");
            }
            else
            {
                if (imageSourceStream != null)
                    imageSourceStream.Dispose();
                MessagingCenter.Send<ProfileMenuPage, Die>(this, "SetDie", selectedDie);

                Navigation.PopAsync();
            }
        }

        private void dieCell_Tapped(object sender, ItemTappedEventArgs args)
        {
            var tappedDieCell = args.Item as Die;
            SetDie(tappedDieCell.Id);
        }

        private void DisableDie()
        {
            FadeOutDieImages(ProfileGrid);
            BuyAppLabel.IsVisible = true;
            BuyAppLabel.Opacity = 1;
            DisableInteraction(ProfileGrid);

            if (selectedDie.IsDefault)
                DeleteDieButton.IsEnabled = false;
        }

        private void EnableDie()
        {
            BuyAppLabel.IsVisible = false;
            EnableInteraction(ProfileGrid);
            ProfileGrid.IsVisible = true;
            ProfileGrid.Opacity = 1;
            FadeInDieImages(ProfileGrid);

            if(selectedDie.IsDefault)
                DeleteDieButton.IsEnabled = false;

        }

        private void SetDie(int dieId)
        {
            EnableInteraction(ProfilePageGrid);
            StopRecording();
            selectedDie = dice.Where(x => x.Id == dieId).FirstOrDefault();
            if(selectedDie.IsDefault)
            {
                
                DieNameIcon.IsVisible = false;
                DeleteDieButton.IsEnabled = false;
				DisableRecordButtons();

                foreach (var dieOptionLayout in ProfileGrid.Children.OfType<StackLayout>())
                {

#if FREE
                if (selectedDie.IsPremium)
                {
                    DisableDie();
                }
                else
                {
                    EnableDie();
                }
#endif
                }
            }
            else
            {
                EnableDie();
                DieNameIcon.IsVisible = true;
                DeleteDieButton.IsEnabled = true;
				EnableRecordButtons();
            }
            DieName.Text = selectedDie.Name;
            SetImages(selectedDie);
            
        }

		private void DisablePlayButtons()
		{
			foreach (var button in PlayButtons)
			{
				button.IsEnabled = false;
				button.Opacity = 0.3;
			}
		}

		private void EnablePlayButtons()
		{
			foreach (var button in PlayButtons)
			{
				button.IsEnabled = true;
				button.Opacity = 1;
			}
		}

		private void DisableRecordButtons()
		{
			foreach (var button in RecordButtons)
			{
				button.IsEnabled = false;
				button.Opacity = 0.3;
			}
		}

		private void EnableRecordButtons()
		{
			foreach (var button in RecordButtons)
			{
				button.IsEnabled = true;
				button.Opacity = 1;
			}
		}

		private IEnumerable<Button> PlayButtons
		{
			get
			{
				// This LINQ query assumes the following.
				// The profile grid contains a number of StackLayouts. The first StackLayout contains
				// the name of the die and we don't care about that. Then there are six StackLayouts,
				// one for each side of the die. These are the ones we want.
				// Within each StackLayout there is another StackLayout that contains two buttons: the
				// play button and the record button, in that order.
				return ProfileGrid.Children.OfType<StackLayout>().Skip(1).Take(6)
					.Select(die => die.Children.OfType<StackLayout>().First())
					.Select(buttonContainer => buttonContainer.Children.OfType<Button>().First());
			}
		}

		private IEnumerable<Button> RecordButtons
		{
			get
			{
				// This LINQ query assumes the following.
				// The profile grid contains a number of StackLayouts. The first StackLayout contains
				// the name of the die and we don't care about that. Then there are six StackLayouts,
				// one for each side of the die. These are the ones we want.
				// Within each StackLayout there is another StackLayout that contains two buttons: the
				// play button and the record button, in that order.
				return ProfileGrid.Children.OfType<StackLayout>().Skip(1).Take(6)
					.Select(die => die.Children.OfType<StackLayout>().First())
					.Select(buttonContainer => buttonContainer.Children.OfType<Button>().Last());
			}
		}

        private void SetImages(Die die)
        {
            var count = 0;
            foreach (var dieOptionLayout in ProfileGrid.Children.OfType<StackLayout>())
            {
                var absoluteLayoutObject = ((StackLayout)dieOptionLayout).Children.OfType<AbsoluteLayout>().FirstOrDefault();
                if (absoluteLayoutObject == null)
                    continue;

                var imageObject = absoluteLayoutObject.Children.OfType<Image>().FirstOrDefault();
                if (imageObject == null)
                    continue;
				
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
                }
				else
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
			
		private async void PlaySound(object sender, EventArgs e)
        {
#if FREE
            if (selectedDie.IsPremium)
            {
                return;
            }

#endif
			DisableRecordButtons();
			DisablePlayButtons();

			try{
	            var playSoundButton = (Button)sender;

	            var dieCount = 0;
	            int.TryParse(playSoundButton.ClassId, out dieCount);
	            var fullpath = "";

	            if(!String.IsNullOrEmpty(selectedDie.Options[dieCount].Sound))
	                fullpath = _pathService.CreateDocumentsPath(selectedDie.Options[dieCount].Sound);

	            if (selectedDie.IsDefault)
	            {
	                fullpath = "Dice/" + selectedDie.Options[dieCount].Sound;
	            }



				if(!String.IsNullOrEmpty(fullpath)){
					if(!_soundService.IsPlaying){
						await _soundService.PlayAsync(fullpath);


					}
				}
					
			}
			catch(Exception ex)
			{
				// What is this catch doing here? Will things break if we remove it?
				// TODO: Find out.
			}
			finally
			{
				// NOTE: this doesn't work because of the bug in PlayAsync().
				EnablePlayButtons();
				EnableRecordButtons();
			}
		}

        private void RecordSound(object sender, EventArgs e)
        {
#if FREE
            if (selectedDie.IsPremium)
            {
                return;
            }
#endif

            _isRecording = true;
            DisableInteraction(ProfilePageGrid);
            _lastRecordSoundButton = (Button)sender;
            _lastRecordSoundButton.IsEnabled = true;

            var imageCount = 0;
            int.TryParse(_lastRecordSoundButton.ClassId, out imageCount);
            _recordingDie = selectedDie;
            _fileStream = _fileManager.OpenFile(String.Format("{0}/{1}.wav", selectedDie.Id, imageCount), FileMode.Create, FileAccess.ReadWrite, FileShare.Write);

			var microphoneService = Resolver.Resolve<IMicrophoneService>();
            _microphone = microphoneService.GetMicrophone();
            _microphone.Start(22050);
            _recorder.StartRecorder(_microphone, _fileStream, 22050);
            _lastRecordSoundButton.Image = "stop.png";

            selectedDie.Options[imageCount].Sound = String.Format("{0}/{1}.wav", selectedDie.Id, imageCount);
            database.SaveDie(selectedDie);

            _lastRecordSoundButton.Clicked -= RecordSound;
            _lastRecordSoundButton.Clicked += StopRecordingButtonClicked;

        }
        
        private void StopRecording()
        {
            if (_isRecording)
            {
                _microphone.Stop();
                _recorder.StopRecorder();
                _lastRecordSoundButton.Image = "record.png";
                _lastRecordSoundButton.Clicked -= StopRecordingButtonClicked;
                _lastRecordSoundButton.Clicked += RecordSound;
                _isRecording = false;
            }
        }

        private void StopRecordingButtonClicked(object sender, EventArgs e)
        {
            _microphone.Stop();
            _recorder.StopRecorder();

            var recordSoundButton = (Button)sender;
            recordSoundButton.Image = "record.png";
            recordSoundButton.Clicked -= StopRecordingButtonClicked;
            recordSoundButton.Clicked += RecordSound;
            _isRecording = false;

            var imageCount = 0;
            int.TryParse(_lastRecordSoundButton.ClassId, out imageCount);

            _fileManager.CreateDirectory(_recordingDie.Id.ToString());
            if (_recordingDie.Options[imageCount].Image == null || _recordingDie.Options[imageCount].Image == "notset.png")
            {
                _recordingDie.Options[imageCount].Image = String.Format("white.png");
            }

            database.SaveDie(_recordingDie);

            SetDie(_recordingDie.Id);

            EnableInteraction(ProfilePageGrid);
        }

        private WaveRecorder _recorder = new WaveRecorder();
        private IFileManager _fileManager;
        private Stream _fileStream;
        private IXFormsApp _app;
        private bool _isRecording;
        private bool _isCameraOpen;
        private Button _lastRecordSoundButton;
        private IAudioStream _microphone;
        private IPathService _pathService;
		private IImageResizerService _imageService;
        private ISoundService _soundService;
        private Die _previousSelectedDie;
        private Die _recordingDie;
        private Stream imageSourceStream;
    }
}
