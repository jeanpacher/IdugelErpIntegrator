using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Media.Imaging;
using Inventor;
using System.IO;

namespace InvUtils
{
    public static class InventorExtensions
    {
        public static BitmapImage GetThumbnailAsBitmapImage(this Document document)
        {
            BitmapImage docThumbnail;
            if (document == null)
            {
                return null;
            }

            try
            {
                if (document.Thumbnail == null)
                {
                    return null;
                }
                Metafile thumbnailMetafile = new Metafile(new IntPtr(document.Thumbnail.Handle), new WmfPlaceableFileHeader());
                Image.GetThumbnailImageAbort imageCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                Bitmap thumbnailBitmap = thumbnailMetafile.GetThumbnailImage(300, 300, imageCallBack, IntPtr.Zero) as Bitmap;
                docThumbnail = ConvertToBitmapImage(thumbnailBitmap);
                //picMeta.Dispose();
                return docThumbnail;
            }
            catch (Exception ex) when (ex is System.Runtime.InteropServices.COMException || ex is ArgumentException)
            {
                return null;
            }
        }

        private static BitmapImage ConvertToBitmapImage(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            MemoryStream editStream = new MemoryStream();
            bitmap.Save(editStream, ImageFormat.Png);
            _ = editStream.Seek(0, SeekOrigin.Begin);
            bitmapImage.StreamSource = editStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        private static bool ThumbnailCallback()
        {
            return false;
        }
    }
}