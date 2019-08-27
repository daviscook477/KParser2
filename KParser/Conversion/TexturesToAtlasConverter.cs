using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Conversion
{
    class TexturesToAtlasConverter
    {
        public Textures.File TexturesFile { get; internal set; }
        public Scml.File ScmlFile { get; internal set; }

        private bool converted = false;
        private Atlas.File atlasFile = null;
        private Build.File buildFile = null;

        public TexturesToAtlasConverter(Textures.File texturesFile, Scml.File scmlFile)
        {
            TexturesFile = texturesFile;
            ScmlFile = scmlFile;
        }

        public Atlas.File GetAtlasFile()
        {
            if (!converted)
            {
                ConvertFile();
                converted = true;
            }
            return atlasFile;
        }

        public Build.File GetBuildFile()
        {
            if (!converted)
            {
                ConvertFile();
                converted = true;
            }
            return buildFile;
        }

        private void ConvertFile()
        {

        }
    }
}
