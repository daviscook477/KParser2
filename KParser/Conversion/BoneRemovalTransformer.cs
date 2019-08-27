using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KParser.Conversion
{
    class BoneRemovalTransformer
    {
        public XmlDocument BaseScml { get; internal set; }

        private bool transformed = false;
        private XmlDocument debonedScml = null;

        public BoneRemovalTransformer(XmlDocument baseScml)
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
            return debonedScml;
        }

        private void TransformFile()
        {

        }
    }
}
