using System;
using AVFoundation;
using System.Threading.Tasks;
using Foundation;
using Xamarin.Forms;
using XLabs.Platform.Services.Media;


[assembly: Dependency(typeof(SoundService))]
namespace Xamarin.Forms.Labs.iOS.Services
{
    public class SoundService : ISoundService
    {

        AVAudioPlayer player;
        bool _isScrubbing = false;

        public SoundService()
        {
        }

        public event EventHandler SoundFileFinished;

        protected virtual void OnFileFinished(SoundFinishedEventArgs e)
        {
            if (SoundFileFinished != null)
            {
                SoundFileFinished(this, e);
            }
        }

        public Task<SoundFile> SetMediaAsync(string filename)
        {
            return Task.Run<SoundFile>(() =>
            {
                _currentFile = new SoundFile();
                _currentFile.Filename = filename;
                NSUrl url = NSUrl.FromFilename(_currentFile.Filename);
                player = AVAudioPlayer.FromUrl(url);
                player.FinishedPlaying += (object sender, AVStatusEventArgs e) =>
                {
                    if (e.Status)
                    {
                        this.OnFileFinished(new SoundFinishedEventArgs(this._currentFile));
                    }
                };
                _currentFile.Duration = TimeSpan.FromSeconds(player.Duration);
                return _currentFile;
            });
        }

        public Task<SoundFile> PlayAsync(string filename, string extension = null)
        {
            return Task.Run<SoundFile>(async () =>
            {
                if (player == null || filename != _currentFile.Filename)
                {
                    await SetMediaAsync(filename);
                }
                player.Play();
                _isPlaying = true;
                return _currentFile;
            });
        }

        public Task GoToAsync(double position)
        {
            return Task.Run(() =>
            {
                if (!_isScrubbing)
                {
                    _isScrubbing = true;
                    player.CurrentTime = position;
                    _isScrubbing = false;
                }
            });
        }

        public double Volume
        {
            get
            {
                if (player == null)
                    return 0.5;
                return (double)player.Volume;
            }
            set
            {
                if (player != null)
                    player.Volume = (float)value;
            }
        }

        private SoundFile _currentFile = null;
        public SoundFile CurrentFile
        {
            get
            {
                return _currentFile;
            }
        }

        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
        }

        public double CurrentTime
        {
            get
            {
                if (player == null)
                    return 0;
                return (double)player.CurrentTime;
            }
        }

        public void Play()
        {
            if (player != null && !_isPlaying)
            {
                player.Play();
                _isPlaying = true;
            }
        }

        public void Stop()
        {
            if (player != null)
            {
                player.Stop();
                _isPlaying = false;
                player.CurrentTime = 0.0;
            }

        }

        public void Pause()
        {
            if (player != null && _isPlaying)
            {
                player.Pause();
                _isPlaying = false;
            }
        }
    }
}