using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Conversion
{
    class Frame
    {
        public float X { get; internal set; }
        public float Y { get; internal set; }
        public float Angle { get; internal set; }
        public float XScale { get; internal set; }
        public float YScale { get; internal set; }

        public Frame(float x, float y, float angle, float xScale, float yScale)
        {
            X = x;
            Y = y;
            Angle = angle;
            XScale = xScale;
            YScale = yScale;
        }
    }
}
