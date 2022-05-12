using ICSharpCode.SharpZipLib.Zip;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keroosha.SuckMySplashUp.Patcher
{
    static internal class ImageExtensions
    { 
        internal static void SaveAsPngToZip(this Image image, ZipFile zip, string entryName)
        {
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, new PngEncoder());

            var imageDataSource = new StaticImageDataSource(memoryStream.ToArray());
            zip.Add(imageDataSource, entryName);
        }
    }
}
