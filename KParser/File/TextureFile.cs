using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace KParser.File
{
    internal class TextureFile : IFile
    {
        public Dictionary<string, Bitmap> NameToBitmap;
        public string OutDir;
        public List<string> OutFiles;

        public TextureFile(string outDir, Dictionary<string, Bitmap> nameToBitmap)
        {
            if (!Directory.Exists(outDir))
                throw new ArgumentException($"The directory specified at {outDir} does not exist!");

            OutDir = outDir;
            NameToBitmap = nameToBitmap;
        }

        public bool WriteFile()
        {
            foreach (var (name, bitmap) in NameToBitmap) bitmap.Save($"{OutDir}/{name}.png");

            return true;
        }
    }
}