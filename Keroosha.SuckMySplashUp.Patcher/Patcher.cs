using ICSharpCode.SharpZipLib.Zip;

namespace Keroosha.SuckMySplashUp.Patcher;

public class Patcher
{
    public string? BinaryPath { get;  }
    public string? SplashPathx2 { get; set; }
    public string? SplashPath { get; set; }

    private string OldFilePath => $"{BinaryPath}.old";
    
    public Patcher(string? binaryPath, string? splashPathx2, string? splashPath = null)
    {
        BinaryPath = binaryPath;
        SplashPathx2 = splashPathx2;
        SplashPath = splashPath;
    }

    private bool FileIsPng(string path)
    {
        var png = new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };   // PNG "\x89PNG\x0D\0xA\0x1A\0x0A"

        var buffer = new byte[8];
        int read;

        using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            read = fs.Read(buffer, 0, 8);
        }

        return read == 8 && buffer.SequenceEqual(png);
    }

    private void EnsurePatcherValid()
    {
        if (string.IsNullOrWhiteSpace(BinaryPath)) 
            throw new ApplicationException("Missing Binary Path");

        if (SplashPathx2 is null && SplashPath is null)
            throw new ApplicationException("Required at least one splash screen");
        // At least one of them is assigned - it's okay I guess
        // TODO make auto resize as: SplashPathx2 => SplashPath
        SplashPath ??= SplashPathx2;
        SplashPathx2 ??= SplashPath;
        if (!File.Exists(SplashPath) || !File.Exists(SplashPathx2))
            throw new ApplicationException("Splash screen files doesn't exist!");
        
        // TODO Re-pack if file is not PNG via Skia
        if (!FileIsPng(SplashPath) || !FileIsPng(SplashPathx2)) 
            throw new ApplicationException("Files should be a png!");
    }

    private void PatchJarWithNewSplashes(ZipFile file)
    {
        foreach (ZipEntry entry in file)
        {
            // We don't care about directories and any file that not splash.png related
            var skip = !(entry.Name.Contains("splash") && entry.Name.EndsWith(".png"));
            if (skip) continue;
            Console.WriteLine($"Patching: {entry.Name}");
            if (entry.Name.Contains("x2"))
            {
                file.Add(SplashPathx2, entry.Name);
                continue;
            }
            file.Add(SplashPath, entry.Name);
        }
    }

    private void BackupOldApp()
    {
        File.Copy(BinaryPath, OldFilePath, true);
        Console.WriteLine($"Old app.jar saved at {OldFilePath}");
    }

    private void RevertOldAppFile()
    {
        Console.WriteLine($"Reverting app.jar");
        File.Move(OldFilePath, BinaryPath, true);
    }
    
    public void PatchJar()
    {
        EnsurePatcherValid();
        // TODO flag to enforce old app.jar rewriting
        BackupOldApp();
        try
        {
            using var jarFile = new ZipFile(BinaryPath);
            if (jarFile.Count is 0) throw new ApplicationException("Jar is empty!");
            jarFile.BeginUpdate();
            PatchJarWithNewSplashes(jarFile);
            jarFile.CommitUpdate();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            RevertOldAppFile();
            throw;
        }
    }
}
