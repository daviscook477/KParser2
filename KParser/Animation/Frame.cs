using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Animation
{
    class Frame
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int Elements { get; set; }
        public List<Element> ElementsList { get; set; }

        public Frame(float x, float y, float width, float height, int elements, List<Element> elementsList)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Elements = elements;
            ElementsList = elementsList;
        }
    }
}
