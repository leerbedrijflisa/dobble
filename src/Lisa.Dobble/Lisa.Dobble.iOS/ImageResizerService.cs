using System;
using System.IO;

#if __IOS__
using Lisa.Dobble.Interfaces;
using System.Drawing;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using CoreImage;
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
            var tempWidth = originalImage.CGImage.Width;
            var tempHeight = originalImage.CGImage.Height;
			var rect = new System.Drawing.Rectangle((int)(tempWidth / 4), (int)(tempHeight / 4), (int)(tempWidth / 2), (int)(tempHeight / 2));
            originalImage = ScaleAndCropImage(originalImage, new SizeF(367, 367));

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

                return originalImage.AsJPEG().ToArray();
            }

        }


        public UIKit.UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            UIKit.UIImage image;
            try
            {
                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception e)
            {
                return null;
            }
            return image;
        }

        public static UIImage ScaleAndCropImage(UIImage sourceImage, SizeF targetSize)
        {
            var imageSize = sourceImage.Size;
            UIImage newImage = null;
            var width = imageSize.Width;
            var height = imageSize.Height;
            var targetWidth = targetSize.Width;
            var targetHeight = targetSize.Height;
            var scaleFactor = 0.0f;
            var scaledWidth = targetWidth;
            var scaledHeight = targetHeight;
            var thumbnailPoint = PointF.Empty;
            if (imageSize != targetSize)
            {
                var widthFactor = targetWidth / width;
                var heightFactor = targetHeight / height;
                if (widthFactor > heightFactor)
                {
					scaleFactor = (float)widthFactor;
                }
                else
                {
					scaleFactor = (float)heightFactor;
                }
				scaledWidth = (float)(width * scaleFactor);
				scaledHeight = (float)(height * scaleFactor);

                // center the image
                if (widthFactor > heightFactor)
                {
                    thumbnailPoint.Y = (targetHeight - scaledHeight) * 0.5f;
                }
                else
                {
                    if (widthFactor < heightFactor)
                    {
                        thumbnailPoint.X = (targetWidth - scaledWidth) * 0.5f;
                    }
                }
            }
            UIGraphics.BeginImageContextWithOptions(targetSize, false, 0.0f);
            var thumbnailRect = new RectangleF(thumbnailPoint, new SizeF(scaledWidth, scaledHeight));
            sourceImage.Draw(thumbnailRect);
            newImage = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();

            return newImage;
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
