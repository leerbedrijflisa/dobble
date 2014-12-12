using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lisa.Dobble
{
    public static class Theme
    {
        public static FileImageSource ButtonImage = Device.OnPlatform(
                        iOS: "info-48.png",
                        Android: "Drawable/info.png",
                        WinPhone: "info.png");
    }
}
