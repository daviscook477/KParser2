using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Build
{
    class File
    {
        public Build Build { get; set; }
        public Dictionary<int, string> HashToName { get; set; }

        public File(Build build, Dictionary<int, string> hashToName)
        {
            Build = build;
            HashToName = hashToName;
        }
    }
}
