using UIKit;
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
        public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);

			// Any kind of touch should roll the die, so whenever a gesture ends -
			// no matter which one - we simulate a swipe.
			FancyFrame frame = (FancyFrame) this.Element;
			frame.SendSwipe();
		}
    }
}
