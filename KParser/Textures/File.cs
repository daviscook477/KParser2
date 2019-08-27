using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace KParser.Textures
{
    class File
    {
        public Dictionary<string, Bitmap> NameToBitmap { get; internal set; }

        public File(Dictionary<string, Bitmap> nameToBitmap)
        {
            NameToBitmap = nameToBitmap;
        }
    }
}
