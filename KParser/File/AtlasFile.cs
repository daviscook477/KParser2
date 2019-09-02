using System.Drawing;
using System.IO;
using KParser;

namespace AnimData
{
    internal class AtlasFile : IFile
    {
        public Bitmap Atlas;

        public string FilePath;

        public AtlasFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"ERROR: the file at {filePath} does not exist!");

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