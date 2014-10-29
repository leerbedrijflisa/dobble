using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Labs.Droid;
using Xamarin.Forms.Labs;
using Xamarin.Forms.Labs.Services;

namespace Lisa.Dobble.Droid
{
    [Activity(Label = "Lisa.Dobble", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : XFormsApplicationDroid
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var container = new SimpleContainer();
            container.Register<IDevice>(t => AndroidDevice.CurrentDevice);

            Resolver.SetResolver(container.GetResolver());

            Xamarin.Forms.Forms.Init(this, bundle);

            SetPage(App.GetMainPage());
        }
    }
}

