using System;
using System.Collections.Generic;
using System.Drawing;

namespace KParser.Textures
{
    class Parser
    {
        public List<string> Paths { get; internal set; }

        private bool loaded = false;
        private File file = null;

        public Parser(List<string> paths)
        {
            foreach(string path in paths)
            {
                if (!System.IO.File.Exists(path))
                {
                    throw new ArgumentException($"The atlas file specified at {path} does not exist!");
                }
            }
            Paths = paths;
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
            Dictionary<string, Bitmap> nameToBitmap = new Dictionary<string, Bitmap>();
            foreach(string path in Paths)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                Bitmap bitmap = new Bitmap(path);
                nameToBitmap.Add(name, bitmap);
            }
            file = new File(nameToBitmap);
        }
    }
}

