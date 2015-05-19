using Lisa.Dobble;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XLabs.Platform.Services.Media;

[assembly: Dependency(typeof(Lisa.Dobble.iOS.MicrophoneService))]
namespace Lisa.Dobble.iOS
{
    class MicrophoneService : IMicrophoneService
    {
        public IAudioStream GetMicrophone()
        {
            return new Microphone();
        }
    }
}
