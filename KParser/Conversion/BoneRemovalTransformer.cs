using System.Xml;

namespace KParser.Conversion
{
    internal class BoneRemovalTransformer
    {
        private readonly XmlDocument debonedScml = null;

        private bool transformed;

        public BoneRemovalTransformer(XmlDocument baseScml)
        {
            BaseScml = baseScml;
        }

        public XmlDocument BaseScml { get; internal set; }

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