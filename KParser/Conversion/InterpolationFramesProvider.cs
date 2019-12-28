using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KParser.Conversion
{
    class InterpolationFramesProvider
    {
        public XmlElement Animation { get; internal set; }
        public Dictionary<int, List<Frame>> FrameBefore { get; internal set; }
        public Dictionary<int, List<Frame>> FrameAfter { get; internal set; }

        public Dictionary<int, Dictionary<int, Frame>> TimelineIdToFrame { get; internal set; }

        public InterpolationFramesProvider(XmlElement animation)
        {
            Animation = animation;
        }
        
    }
}
