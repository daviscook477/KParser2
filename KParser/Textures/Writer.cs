using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace KParser.Textures
{
    class Writer
    {
        public string Path { get; internal set; }
        public File File { get; internal set; }

        private bool written = false;
        
        public Writer(string path, File file)
        {
            Path = path;
            if (!System.IO.Directory.Exists(path))
            {
                throw new ArgumentException($"The directory specified at {path} does not exist!");
            }

            File = file;
        }

        public void WriteFile()
        {
            if (!written)
            {
                WriteFileInternal();
                written = true;
            }
        }

        private void WriteFileInternal()
        {
            foreach (KeyValuePair<string, Bitmap> entry in File.NameToBitmap)
            {
                string path = $"{Path}/{entry.Key}.png";
                entry.Value.Save(path);
            }
        }
    }
}
