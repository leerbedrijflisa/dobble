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
        public static FileImageSource CreditsButton = Device.OnPlatform(
                        iOS: "info-48.png",
                        Android: "Drawable/info.png",
                        WinPhone: "info.png");
        public static ImageSource Logo = Device.OnPlatform(
                        iOS: "Logo.png",
                        Android: "Drawable/logo.png",
                        WinPhone: "Logo.png");
        public static ImageSource DobbleMask = Device.OnPlatform(
                        iOS:"dobblemask_small.png",
                        Android:"Drawable/dobblemasksmall.png",
                        WinPhone:"dobblemasksmall.png");
        public static FileImageSource PlayButton = Device.OnPlatform(
                       iOS: "play.png",
                       Android: "Drawable/play.png",
                       WinPhone: "play.png");
        public static FileImageSource RecordButton = Device.OnPlatform(
                       iOS: "record.png",
                       Android: "Drawable/record.png",
                       WinPhone: "record.png");
        public static ImageSource Timer = Device.OnPlatform(
                        iOS: "tick.png",
                        Android: "Drawable/tick.png",
                        WinPhone: "tick.png");
    }
}
