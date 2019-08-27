using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Animation
{
    class Animation
    {
        public int Version { get; set; }
        public int Elements { get; set; }
        public int Frames { get; set; }
        public int Animations { get; set; }
        public List<Bank> BanksList { get; set; }
        public int MaxVisSymbolFrames { get; set; }

        public Animation(int version, int elements, int frames, int animations, List<Bank> banksList, int maxVisSymbolFrames)
        {
            Version = version;
            Elements = elements;
            Frames = frames;
            Animations = animations;
            BanksList = banksList;
            MaxVisSymbolFrames = maxVisSymbolFrames;
        }
    }
}
