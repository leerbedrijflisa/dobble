using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Labs.Services.Media;

namespace Lisa.Dobble
{
    public interface IMicrophoneService
    {
        IAudioStream GetMicrophone();
    }
}