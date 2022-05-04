using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace Keroosha.SuckMySplashUp.Patcher
{
    internal class ImageUtils
    {
        internal static byte[] ConvertImageToByteArray(Image image, IImageEncoder encoder)
        {
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, encoder);
            return memoryStream.ToArray();
        }

        internal static void SaveImageToZipAs(Image image, ZipFile zip, IImageEncoder encoder, string entryName)
        {
            var imageDataSource = new StaticImageDataSource(ConvertImageToByteArray(image, encoder));
            zip.Add(imageDataSource, entryName);
        }
        internal static bool FileIsImage(string path) => Image.Identify(path) != null;

    }
}
