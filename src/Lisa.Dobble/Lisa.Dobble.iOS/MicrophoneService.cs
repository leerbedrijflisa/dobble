using Lisa.Dobble;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Labs.iOS.Services.Media;
using Xamarin.Forms.Labs.Services.Media;

[assembly: Dependency(typeof(SoundTest.iOS.MicrophoneService))]
namespace SoundTest.iOS
{
    class MicrophoneService : IMicrophoneService
    {
        public IAudioStream GetMicrophone()
        {
            return new Microphone();
        }
    }
}
