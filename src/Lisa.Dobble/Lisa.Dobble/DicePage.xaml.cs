using Lisa.Dobble.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lisa.Dobble
{
    public partial class DicePage
    {
        private Dice dice;
        public DicePage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            CreateSampleDice();

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.TappedCallback += (s, e) =>
            {
                RollDice();
            };

            MainGrid.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void CreateSampleDice()
        {
            dice = new Dice();
            dice.Options = new List<Option>();

            for(var i = 0; i < 6; i++)
            {
                var option = new Option();

                if(i == 0)
                {
                    option.Image = "dice.png";
                }else{
                    option.Image = String.Format("dice{0}.png", i + 1);
                }

                dice.Options.Add(option);
            }

            dice.Name = "Sample";
        }

        private void RollDice()
        {
            var random = new Random();
            int randomNumber = random.Next(0, dice.Options.Count());

            DiceImage.Source = dice.Options[randomNumber].Image;
            this.InvalidateMeasure();
        }
    }
}
