using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KParser.Scml.Alternate
{
    /* provides sequential 0-indexed integers
     * that are used as ids for the timelines
     * so they can be arranged in an array */
    class SpriteSymbolIdProvider
    {
        public Scml.File ScmlFile { get; set; }

        public string Name { get; set; }

        public Dictionary<int, int> TimelineIdMap { get; set; }

        public Dictionary<int, KParser.Conversion.ChildType> TimelineTypeMap { get; set; }

        private int nextIndex = 0;

        public SpriteSymbolIdProvider(Scml.File scmlFile, string name)
        {
            ScmlFile = scmlFile;
            Name = name;
            TimelineIdMap = new Dictionary<int, int>();
            TimelineTypeMap = new Dictionary<int, KParser.Conversion.ChildType>();
            BuildTimelineIdMap();
        }

        /* get the assigned id for a specific timeline
         * this id is the value that should be used for the id field any time
         * this timeline is referenced */
        public int GetId(int timelineId)
        {
            return TimelineIdMap[timelineId];
        }

        /* get the assigned type for a specific timeline
         * each timeline will either represent a bone for all of its frames
         * or a sprite for all of its frames - never can change for the duration
         * of just a single timeline */
        public KParser.Conversion.ChildType GetType(int timelineId)
        {
            return TimelineTypeMap[timelineId];
        }

        /* gets the amount of ids that are maintained
         * should be used as the size of the array to know
         * how many elements it needs to contain each timeline
         * information */
        public int Size()
        {
            return nextIndex;
        }

        private void BuildTimelineIdMap()
        {
            XmlElement spriterData = (XmlElement)ScmlFile.Scml.GetElementsByTagName("spriter_data")[0];
            XmlElement entity = GetFirstChildByName(spriterData, "entity");
            XmlElement animation = GetFirstChildByAttribute(entity, "name", Name);
            XmlElement mainline = GetFirstChildByName(animation, "mainline");
            foreach (XmlNode keyNode in mainline.ChildNodes)
            {
                foreach (XmlNode refNode in keyNode.ChildNodes)
                {
                    if (refNode is XmlElement && 
                        (refNode.Name.Equals("object_ref") || refNode.Name.Equals("bone_ref")))
                    {
                        int timeline = int.Parse(((XmlElement)refNode).GetAttribute("timeline"));
                        /* assign next index to this timeline id if it has not previously been assigned a value */
                        if (!TimelineIdMap.ContainsKey(timeline))
                        {
                            TimelineIdMap[timeline] = nextIndex++;

                            /* set the type of the timeline to either sprite or bone based on which is the first seen for the timline
                             * since the timeline type is invariant across frames */
                            TimelineTypeMap[timeline] = KParser.Conversion.ChildType.Sprite;
                            if (refNode.Name.Equals("bone_ref"))
                            {
                                TimelineTypeMap[timeline] = KParser.Conversion.ChildType.Bone;
                            }
                        }
                    }
                }
            }
        }

        private XmlElement GetFirstChildByName(XmlElement parent, string tagName)
        {
            foreach (XmlNode node in parent.ChildNodes)
            {
                if (node is XmlElement && node.Name.Equals(tagName))
                {
                    return (XmlElement)node;
                }
            }
            return null;
        }

        private XmlElement GetFirstChildByAttribute(XmlElement parent, string attributeName, string attributeValue)
        {
            foreach (XmlNode node in parent.ChildNodes)
            {
                if (node is XmlElement)
                {
                    XmlElement element = (XmlElement)node;
                    if (element.HasAttribute(attributeName) && element.GetAttribute(attributeName) == attributeValue)
                    {
                        return element;
                    }
                }
            }
            return null;
        }

    }
}
