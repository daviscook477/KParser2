using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KParser.Conversion
{
    class TimeStepFrameSaturationConverter
    {
        public XmlDocument BaseScml { get; internal set; }

        private bool transformed = false;
        private XmlDocument saturatedScml = null;

        public TimeStepFrameSaturationConverter(XmlDocument baseScml)
        {
            BaseScml = baseScml;
        }

        public XmlDocument GetDebonedScml()
        {
            if (!transformed)
            {
                TransformFile();
                transformed = true;
            }
            return saturatedScml;
        }

        private void TransformFile()
        {

        }

        private void InterpolateFrames(string animationName, int startId, int endId, int refId, ChildType type)
        {

        }

        private XmlElement GetRef(string animationName, int id, int refId, ChildType type)
        {
            XmlElement spriterData = (XmlElement)BaseScml.GetElementsByTagName("spriter_data")[0];
            XmlElement entity = GetFirstChildByName(spriterData, "entity");
            XmlElement animation = GetFirstChildByAttribute(entity, "name", animationName);
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
