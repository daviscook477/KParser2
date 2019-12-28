using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Scml.Alternate
{
    class File
    {
        public string Name { get; set; }

        public List<Animation> Animations { get; set; }

        public File(string name, List<Animation> animations)
        {
            Name = name;
            Animations = animations;
        }
    }
}
