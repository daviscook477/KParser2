using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Animation
{
    class File
    {
        public Animation Animation { get; set; }
        public Dictionary<int, string> HashToName { get; set; }

        public File(Animation animation, Dictionary<int, string> hashToName)
        {
            Animation = animation;
            HashToName = hashToName;
        }
    }
}
