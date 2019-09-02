using System.Drawing;
using System.IO;

namespace KParser.File
{
    internal class AtlasFile : IFile
    {
        public Bitmap Atlas;

        public string FilePath;

        public AtlasFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                throw new FileNotFoundException($"The file at {filePath} does not exist!");

            FilePath = filePath;
            Atlas = new Bitmap(FilePath);
        }

        public bool WriteFile()
        {
            Atlas.Save(FilePath);
            return true;
        }
    }
}