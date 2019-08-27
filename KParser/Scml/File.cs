using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KParser.Scml
{
    class File
    {
        public XmlDocument Scml { get; internal set; }

        public File(XmlDocument scml)
        {
            Scml = scml;
        }
    }
}
