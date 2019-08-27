using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace KParser.Atlas
{
    class Parser
    {
        public string Path { get; internal set; }

        private bool loaded = false;
        private File file = null;

        public Parser(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                throw new ArgumentException($"The atlas file specified at {path} does not exist!");
            }
            Path = path;
        }

        public File GetFile()
        {
            if (!loaded)
            {
                LoadFile();
                loaded = true;
            }
            return file;
        }

        private void LoadFile()
        {
            Bitmap atlas = new Bitmap(Path);
            file = new File(atlas);
        }
    }
}
