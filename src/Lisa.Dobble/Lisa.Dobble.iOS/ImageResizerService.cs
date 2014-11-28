using System;
using System.IO;

#if __IOS__
using Lisa.Dobble.Interfaces;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using Xamarin.Forms;
#endif

#if __ANDROID__
using Android.Graphics;
#endif

#if WINDOWS_PHONE
using Microsoft.Phone;
using System.Windows.Media.Imaging;
#endif

[assembly: Dependency(typeof(Lisa.Dobble.ImageResizerService))]
namespace Lisa.Dobble
{
    public class ImageResizerService : IImageResizerService
    {
        public ImageResizerService()
        {
        }

        public byte[] ResizeImage(byte[] imageData, float width, float height)
        {
#if __IOS__
            return ResizeImageIOS(imageData, width, height);
#endif
#if __ANDROID__
			return ResizeImageAndroid ( imageData, width, height );
#endif
#if WINDOWS_PHONE
			return ResizeImageWinPhone ( imageData, width, height );
#endif
        }


#if __IOS__
        public byte[] ResizeImageIOS(byte[] imageData, float width, float height)
        {
            UIImage originalImage = ImageFromByteArray(imageData);

            //create a 24bit RGB image
            using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                (int)width, (int)height, 8,
                (int)(4 * width), CGColorSpace.CreateDeviceRGB(),
                CGImageAlphaInfo.PremultipliedFirst))
            {
                switch (originalImage.Orientation)
                {
                    case UIImageOrientation.Left:
                        context.RotateCTM((float)Math.PI / 2);
                        context.TranslateCTM(0, -height);
                        break;
                    case UIImageOrientation.Right:
                        context.RotateCTM(-((float)Math.PI / 2));
                        context.TranslateCTM(-width, 0);
                        break;
                    case UIImageOrientation.Up:
                        break;
                    case UIImageOrientation.Down:
                        context.TranslateCTM(width, height);
                        context.RotateCTM(-(float)Math.PI);
                        break;
                }

                RectangleF imageRect = new RectangleF(0, 0, width, height);

                // draw the image
                context.DrawImage(imageRect, originalImage.CGImage);

                MonoTouch.UIKit.UIImage resizedImage = MonoTouch.UIKit.UIImage.FromImage(context.ToImage());
                
                // save the image as a jpeg
                return resizedImage.AsJPEG().ToArray();
            }
        }

        public MonoTouch.UIKit.UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            MonoTouch.UIKit.UIImage image;
            try
            {
                image = new MonoTouch.UIKit.UIImage(MonoTouch.Foundation.NSData.FromArray(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Image load failed: " + e.Message);
                return null;
            }
            return image;
        }
#endif

#if __ANDROID__

		public static byte[] ResizeImageAndroid (byte[] imageData, float width, float height)
		{
			// Load the bitmap
			Bitmap originalImage = BitmapFactory.DecodeByteArray (imageData, 0, imageData.Length);
			Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);

			using (MemoryStream ms = new MemoryStream())
			{
				resizedImage.Compress (Bitmap.CompressFormat.Jpeg, 100, ms);
				return ms.ToArray ();
			}
		}

#endif

#if WINDOWS_PHONE

        public static byte[] ResizeImageWinPhone (byte[] imageData, float width, float height)
        {
            byte[] resizedData;

            using (MemoryStream streamIn = new MemoryStream (imageData))
            {
                WriteableBitmap bitmap = PictureDecoder.DecodeJpeg (streamIn, (int)width, (int)height);

                using (MemoryStream streamOut = new MemoryStream ())
                {
                    bitmap.SaveJpeg(streamOut, (int)width, (int)height, 0, 100);
                    resizedData = streamOut.ToArray();
                }
            }
            return resizedData;
        }
        
#endif

    }
}
