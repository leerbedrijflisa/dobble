using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Lisa.Dobble;
using Lisa.Dobble.iOS;

[assembly: ExportRenderer(typeof(FancyFrame), typeof(FancyIosFrameRenderer))]
namespace Lisa.Dobble.iOS
{
    public class FancyIosFrameRenderer : FrameRenderer
    {
        FancyFrame frame;

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            frame = (FancyFrame)this.Element;
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

            var swipeGestureRecognizer7 = new UISwipeGestureRecognizer(HandleSwipe);
            swipeGestureRecognizer7.Direction = UISwipeGestureRecognizerDirection.Down | UISwipeGestureRecognizerDirection.Up;
            swipeGestureRecognizer7.NumberOfTouchesRequired = 4;

            var swipeGestureRecognizer8 = new UISwipeGestureRecognizer(HandleSwipe);
            swipeGestureRecognizer8.Direction = UISwipeGestureRecognizerDirection.Left | UISwipeGestureRecognizerDirection.Right;
            swipeGestureRecognizer8.NumberOfTouchesRequired = 42;


            this.AddGestureRecognizer(swipeGestureRecognizer);
            this.AddGestureRecognizer(swipeGestureRecognizer2);
            this.AddGestureRecognizer(swipeGestureRecognizer3);
            this.AddGestureRecognizer(swipeGestureRecognizer4);
            this.AddGestureRecognizer(swipeGestureRecognizer5);
            this.AddGestureRecognizer(swipeGestureRecognizer6);
            this.AddGestureRecognizer(swipeGestureRecognizer7);
            this.AddGestureRecognizer(swipeGestureRecognizer8);
        }

        private void HandleSwipe()
        {
            frame.SendSwipe();
        }
    }
}
