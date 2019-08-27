using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Build
{
    class Symbol
    {
        public int Hash { get; set; }
        public int Path { get; set; }
        public int Color { get; set; }
        public int Flags { get; set; }
        public int NumFrames { get; set; }
        public List<Frame> FramesList { get; set; }

        public Symbol(int hash, int path, int color, int flags, int numFrames, List<Frame> framesList)
        {
            Hash = hash;
            Path = path;
            Color = color;
            Flags = flags;
            NumFrames = numFrames;
            FramesList = framesList;
        }
    }
}
