using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lisa.Dobble;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Forms.Platform.iOS;
using Lisa.Dobble.iOS;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(FancyImage), typeof(FancyIosImageRenderer))]
namespace Lisa.Dobble.iOS
{
    class FancyIosImageRenderer : ImageRenderer
    {

        FancyImage image;

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            image = (FancyImage)this.Element;
            //var tapGestureRecognizer = new UITapGestureRecognizer(HandleSwipe);

            var swipeGestureRecognizer = new UISwipeGestureRecognizer(HandleSwipe);
            swipeGestureRecognizer.Direction = UISwipeGestureRecognizerDirection.Down | UISwipeGestureRecognizerDirection.Up;

            var swipeGestureRecognizer2 = new UISwipeGestureRecognizer(HandleSwipe);
            swipeGestureRecognizer2.Direction = UISwipeGestureRecognizerDirection.Left | UISwipeGestureRecognizerDirection.Right;

            var swipeGestureRecognizer3 = new UISwipeGestureRecognizer(HandleSwipe);
            swipeGestureRecognizer3.Direction = UISwipeGestureRecognizerDirection.Down | UISwipeGestureRecognizerDirection.Up;
            swipeGestureRecognizer3.NumberOfTouchesRequired = 2;

            var swipeGestureRecognizer4 = new UISwipeGestureRecognizer(HandleSwipe);
            swipeGestureRecognizer4.Direction = UISwipeGestureRecognizerDirection.Left | UISwipeGestureRecognizerDirection.Right;
            swipeGestureRecognizer4.NumberOfTouchesRequired = 2;

            var swipeGestureRecognizer5 = new UISwipeGestureRecognizer(HandleSwipe);
            swipeGestureRecognizer5.Direction = UISwipeGestureRecognizerDirection.Down | UISwipeGestureRecognizerDirection.Up;
            swipeGestureRecognizer5.NumberOfTouchesRequired = 3;

            var swipeGestureRecognizer6 = new UISwipeGestureRecognizer(HandleSwipe);
            swipeGestureRecognizer6.Direction = UISwipeGestureRecognizerDirection.Left | UISwipeGestureRecognizerDirection.Right;
            swipeGestureRecognizer6.NumberOfTouchesRequired = 3;

            this.AddGestureRecognizer(swipeGestureRecognizer);
            this.AddGestureRecognizer(swipeGestureRecognizer2);
            this.AddGestureRecognizer(swipeGestureRecognizer3);
            this.AddGestureRecognizer(swipeGestureRecognizer4);
            this.AddGestureRecognizer(swipeGestureRecognizer5);
            this.AddGestureRecognizer(swipeGestureRecognizer6);
        }

        private void HandleSwipe()
        {
            image.SendSwipe();
        }
    }
}