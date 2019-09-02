using System.Collections.Generic;

namespace KParser.Build
{
    internal struct BuildData
    {
        public Build Build;
        public Dictionary<int, string> HashToName;
    }

    internal struct Frame
    {
        public int SourceFrameNum;
        public int Duration;
        public int BuildImageIdx;
        public float PivotX;
        public float PivotY;
        public float PivotWidth;
        public float PivotHeight;
        public float X1;
        public float Y1;
        public float X2;
        public float Y2;
        public int Time;
    }

    internal struct Symbol
    {
        public int Hash;
        public int Path;
        public int Color;
        public int Flags;
        public int NumFrames;
        public List<Frame> FramesList;
    }

    internal struct Build
    {
        public int Version;
        public int SymbolCount;
        public int FrameCount;
        public string Name;
        public List<Symbol> SymbolsList;
    }
}
