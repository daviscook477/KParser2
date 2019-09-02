using System;
using System.Xml;

namespace KParser.File
{
    internal class ScmlFile : IFile
    {
        public string FilePath;
        public XmlDocument ScmlDocument;

        public ScmlFile(string filePath, XmlDocument scmlData)
        {
            if (System.IO.File.Exists(filePath))
                Console.WriteLine($"Warning: the file at {filePath} already exists! Overwriting {filePath}...");

            FilePath = filePath;
            ScmlDocument = scmlData;
        }

        public bool WriteFile()
        {
            try
            {
                ScmlDocument.Save(FilePath);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e);
                return false;
            }

            return false;
        }
    }
}