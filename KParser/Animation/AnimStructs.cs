using System.Collections.Generic;

namespace KParser.Animation
{
    internal struct Animation
    {
        public int Version;
        public int ElementCount;
        public int FrameCount;
        public int AnimationCount;
        public List<Bank> BanksList;
        public int MaxVisSymbolFrames;
    }

    internal struct Bank
    {
        public string Name;
        public int Hash;
        public float Rate;
        public int FrameCount;
        public List<Frame> FramesList;
    }

    // We never use the HashCode of this object
    // If this object is ever added to a Dictionary or other HashSet<T>
    // it will be necessary to implement GetHashCode in some way
#pragma warning disable 660,661,659
    internal struct Frame
#pragma warning restore 660,661,659
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public int ElementCount;
        public List<Element> ElementsList;


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            var o = (Frame) obj;
            return X == o.X && Y == o.Y && Width == o.Width && Height == o.Height &&
                   ElementCount == o.ElementCount && ElementsList == o.ElementsList;
        }

        public static bool operator ==(Frame a, Frame b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Frame a, Frame b)
        {
            return !a.Equals(b);
        }
    }

    internal struct Element
    {
        public int Image;
        public int Index;
        public int Layer;
        public int Flags;
        public float R;
        public float G;
        public float B;
        public float A;
        public float M1;
        public float M2;
        public float M3;
        public float M4;
        public float M5;
        public float M6;
        public float Order;
        public int ZIndex;
    }

    internal struct AnimData
    {
        public Animation Animation;
        public Dictionary<int, string> HashToName;
    }
}