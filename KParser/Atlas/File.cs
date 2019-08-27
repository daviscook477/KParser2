using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace KParser.Atlas
{
    class File
    {
        public Bitmap Atlas { get; internal set; }

        public File(Bitmap atlas)
        {
            Atlas = atlas;
        }
    }
}
