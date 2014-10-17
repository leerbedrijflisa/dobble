using Lisa.Dobble.Data;
using Lisa.Dobble.Models;
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
        DieDatabase database;
        IEnumerable<Die> dice;
        public ProfileMenuPage()
        {
            InitializeComponent();
            database = new DieDatabase();
            dice = database.GetDice();
            ProfileListView.ItemTapped += dieCell_Tapped;
            ProfileListView.ItemsSource = dice;
        }

        private void dieCell_Tapped(object sender, ItemTappedEventArgs args)
        {
            var tappedDieCell = args.Item as Die;
            SetDie(tappedDieCell.Id);
        }

        private void SetDie(int dieId)
        {
            var selectedDie = dice.Where(x => x.Id == dieId).FirstOrDefault();
            SetImages(selectedDie);
        }

        private void SetImages(Die die)
        {
            var count = 1;
            foreach (var dieOptionLayout in ProfileGrid.Children.OfType<StackLayout>())
            {
                var imageObject = ((StackLayout)dieOptionLayout).Children.OfType<Image>().FirstOrDefault();
                var dieImage = (Image)imageObject;
                dieImage.Source = Device.OnPlatform(
                    iOS: ImageSource.FromFile("Dice/dice" + count + ".png"),
                    Android: ImageSource.FromFile("Drawable/dice" + count + ".png"),
                    WinPhone: ImageSource.FromFile("dice" + count + ".png"));

                count++;
            }
        }
    }
}
