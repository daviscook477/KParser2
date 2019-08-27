using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Animation
{
    class Element
    {
        public int Image { get; set; }
        public int Index { get; set; }
        public int Layer { get; set; }
        public int Flags { get; set; }
        public float A { get; set; }
        public float B { get; set; }
        public float G { get; set; }
        public float R { get; set; }
        public float M1 { get; set; }
        public float M2 { get; set; }
        public float M3 { get; set; }
        public float M4 { get; set; }
        public float M5 { get; set; }
        public float M6 { get; set; }
        public float Order { get; set; }
        public int ZIndex { get; set; }

        public Element(int image, int index, int layer, int flags, float a, float b, float g, float r, float m1, float m2, float m3, float m4, float m5, float m6, float order) : 
            this(image, index, layer, flags, a, b, g, r, m1, m2, m3, m4, m5, m6, order, -1) { }

        public Element(int image, int index, int layer, int flags, float a, float b, float g, float r, float m1, float m2, float m3, float m4, float m5, float m6, float order, int zIndex)
        {
            Image = image;
            Index = index;
            Layer = layer;
            Flags = flags;
            A = a;
            B = b;
            G = g;
            R = r;
            M1 = m1;
            M2 = m2;
            M3 = m3;
            M4 = m4;
            M5 = m5;
            M6 = m6;
            Order = order;
            ZIndex = zIndex;
        }
    }
}
