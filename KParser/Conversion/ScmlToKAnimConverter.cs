using KParser.File;

namespace KParser.Conversion
{
    internal class ScmlToKAnimConverter
    {
        private readonly AnimFile animationFile = null;
        private AtlasFile atlasFile;
        private BuildFile buildFile;

        private bool converted;
        public ScmlFile ScmlFile;

        public TextureFile TexturesFile;

        public ScmlToKAnimConverter(TextureFile texturesFile, ScmlFile scmlFile)
        {
            TexturesFile = texturesFile;
            ScmlFile = scmlFile;
        }

        public AtlasFile GetAtlasFile()
        {
            if (!converted)
            {
                ConvertFile();
                converted = true;
            }

            return atlasFile;
        }

        public BuildFile GetBuildFile()
        {
            if (!converted)
            {
                ConvertFile();
                converted = true;
            }

            return buildFile;
        }

        public AnimFile GetAnimationFile()
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
            var texturesToAtlasConverter = new TexturesToAtlasConverter(TexturesFile, ScmlFile);
            atlasFile = texturesToAtlasConverter.GetAtlasFile();
            buildFile = texturesToAtlasConverter.GetBuildFile();
        }
    }
}