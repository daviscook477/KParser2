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
    }
}
