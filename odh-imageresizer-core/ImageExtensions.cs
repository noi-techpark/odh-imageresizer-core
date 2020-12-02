using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace odh_imageresizer_core
{
    public static class ImageExtensions
    {
        public static string GetMimeType(this ImageFormat imageFormat)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.First(codec => codec.FormatID == imageFormat.Guid).MimeType;
        }

        private static string GetMimeType(this Image image)
        {
            return image.RawFormat.GetMimeType();
        }
    }
}
