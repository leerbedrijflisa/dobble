using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLabs.Platform.Services.Media;

namespace Lisa.Dobble
{
    public interface IMicrophoneService
    {
        IAudioStream GetMicrophone();
    }
}