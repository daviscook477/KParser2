using System.Xml;

namespace KParser.Conversion
{
    internal class TimeStepFrameSaturationConverter
    {
        private readonly XmlDocument saturatedScml = null;

        private bool transformed;

        public TimeStepFrameSaturationConverter(XmlDocument baseScml)
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

            return saturatedScml;
        }

        private void TransformFile()
        {
        }
    }
}