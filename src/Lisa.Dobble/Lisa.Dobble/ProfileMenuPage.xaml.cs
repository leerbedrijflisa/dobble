using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lisa.Dobble
{
    public partial class ProfileMenuPage
    {
        public ProfileMenuPage()
        {
            InitializeComponent();

            diceImage.Source =  Device.OnPlatform(
                iOS: ImageSource.FromFile("Dice/dice.png"),
                Android:  ImageSource.FromFile("Drawable/dice.png"),
                WinPhone: ImageSource.FromFile("dice.png"));
        }
    }
}
