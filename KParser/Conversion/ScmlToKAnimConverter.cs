using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Conversion
{
    class ScmlToKAnimConverter
    {
        public Textures.File TexturesFile { get; internal set; }
        public Scml.File ScmlFile { get; internal set; }

        private bool converted = false;
        private Atlas.File atlasFile = null;
        private Build.File buildFile = null;
        private Animation.File animationFile = null;

        public ScmlToKAnimConverter(Textures.File texturesFile, Scml.File scmlFile)
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

        public Animation.File GetAnimationFile()
        {
            if (!converted)
            {
                ConvertFile();
                converted = true;
            }
            return animationFile;
        }

        private void ConvertFile()
        {
            TexturesToAtlasConverter texturesToAtlasConverter = new TexturesToAtlasConverter(TexturesFile, ScmlFile);
            atlasFile = texturesToAtlasConverter.GetAtlasFile();
            buildFile = texturesToAtlasConverter.GetBuildFile();
        }
    }
}
