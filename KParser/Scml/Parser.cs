using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KParser.Scml
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
                throw new ArgumentException($"The scml file specified at {path} does not exist!");
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
            XmlDocument scml = new XmlDocument();
            scml.Load(Path);
            file = new File(scml);
        }
    }
}
