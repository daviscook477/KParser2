using AnimData;
using KParser.File;

namespace KParser.Conversion
{
    internal class TexturesToAtlasConverter
    {
        private AtlasFile atlasFile = null;
        private BuildFile buildFile = null;

        public TexturesToAtlasConverter(TextureFile texturesFile, ScmlFile scmlFile)
        {
            TexturesFile = texturesFile;
            ScmlFile = scmlFile;
        }

        public TextureFile TexturesFile { get; internal set; }
        public ScmlFile ScmlFile { get; internal set; }

        public AtlasFile GetAtlasFile()
        {
            return atlasFile;
        }

        public BuildFile GetBuildFile()
        {
            return buildFile;
        }
    }
}