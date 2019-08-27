using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Scml
{
    class Writer
    {
        public string Path { get; internal set; }
        public File File { get; internal set; }

        private bool written = false;

        public Writer(string path, File file)
        {
            Path = path;
            if (System.IO.File.Exists(path))
            {
                Console.WriteLine($"Warning: the file at {path} already exists! Overwriting {path}...");
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
            File.Scml.Save(Path);
        }
    }
}
