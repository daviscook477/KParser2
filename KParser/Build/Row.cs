using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Build
{
    class Row
    {
        public Build Build { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
        public int Hash { get; set; }
        public int Time { get; set; }
        public int Duration { get; set; }
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float PivotX { get; set; }
        public float PivotY { get; set; }
        public float PivotWidth { get; set; }
        public float PivotHeight { get; set; }
    }
}
