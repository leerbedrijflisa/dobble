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
            foreach(Die die in dice)
            {
                TextCell dieCell = new TextCell();
                dieCell.Text = die.Name;
                dieCell.ClassId = die.Id.ToString();
                dieCell.Tapped += dieCell_Tapped;
                ProfileSection.Add(dieCell);
            }
        }

        private void dieCell_Tapped(object sender, EventArgs e)
        {
            var tappedDieCell = (TextCell)sender;
            SetDie(int.Parse(tappedDieCell.ClassId));
        }

        private void SetDie(int dieId)
        {
            var selectedDie = dice.Where(x => x.Id == dieId).FirstOrDefault();
            SetImages(selectedDie);
        }

        private void SetImages(Die die)
        {
            var count = 1;
            foreach(var imageObject in ProfileGrid.Children.OfType<Image>())
            {
                var dieImage = (Image)imageObject;
                dieImage.Source = Device.OnPlatform (
                    iOS: ImageSource.FromFile("Dice/dice" + count + ".png"),
                    Android: ImageSource.FromFile("Drawable/dice"  + count + ".png"),
                    WinPhone: ImageSource.FromFile("dice" + count + ".png"));

                count++;
            }
        }
    }
}
