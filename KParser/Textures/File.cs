using System.Collections.Generic;
using System.Drawing;

namespace KParser.Textures
{
    internal class File1
    {
        public File1(Dictionary<string, Bitmap> nameToBitmap)
        {
            NameToBitmap = nameToBitmap;
        }

        public Dictionary<string, Bitmap> NameToBitmap { get; internal set; }
    }
}