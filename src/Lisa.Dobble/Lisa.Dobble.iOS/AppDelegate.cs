using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;
using XLabs.Platform.Mvvm;
using XLabs.Forms;
using XLabs.Platform.Services.IO;
using Acr.UserDialogs;
using Lisa.Dobble.Interfaces;


namespace Lisa.Dobble.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        // class-level declarations
        UIWindow window;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			RegisterServices();
            app.Init();
            Forms.Init();
            
            window = new UIWindow(UIScreen.MainScreen.Bounds);

            window.RootViewController = App.GetMainPage().CreateViewController();

            window.MakeKeyAndVisible();

            return true;
        }

        private void RegisterServices()
        {
            var resolverContainer = new SimpleContainer();
			var app = new XFormsAppiOS();
			UserDialogs.Init();

			resolverContainer
				.Register<IXFormsApp>(app)
				.Register<IDevice>(t => AppleDevice.CurrentDevice)
				.Register<IMediaPicker, MediaPicker>()
				.Register<ISoundService, Lisa.Dobble.iOS.SoundService>()
				.Register<IMicrophoneService, Lisa.Dobble.iOS.MicrophoneService>()
				.Register<IFileManager, Lisa.Dobble.iOS.FileManager>()
				.Register<IPathService, Lisa.Dobble.iOS.PathService>()
				.Register<IImageResizerService, Lisa.Dobble.iOS.ImageResizerService>()
				.Register<IUserDialogs>(UserDialogs.Instance);
			
            Resolver.SetResolver(resolverContainer.GetResolver());
        }
    }
}
