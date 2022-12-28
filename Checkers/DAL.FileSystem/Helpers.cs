namespace DAL.FileSystem;

public class Helpers
{
    public static string GetFileName(string directory, string filename, string fileExtension)
    {
        return directory +
               Path.DirectorySeparatorChar +
               filename + "." + fileExtension;
    }

    public static void CheckOrCreateDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}