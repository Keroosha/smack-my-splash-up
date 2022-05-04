using ICSharpCode.SharpZipLib.Zip;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace Keroosha.SuckMySplashUp.Patcher;

public class StaticImageDataSource : IStaticDataSource
{

    private readonly byte[] _image;

    public StaticImageDataSource(byte[] image)
    {
        _image = image;
    }
    public Stream GetSource()
    {
        return new MemoryStream(_image);
    }
}