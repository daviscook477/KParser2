using System;
using System.Collections.Generic;
using System.Drawing;
using KParser.File;

namespace KParser.Conversion
{
    internal class AtlasToTexturesConverter
    {
        public AtlasFile AtlasFile;
        public BuildFile BuildFile;

        private bool convertedTextures;
        public string OutDir;
        private TextureFile texturesFile;

        public AtlasToTexturesConverter(AtlasFile atlasFile, BuildFile buildFile, string outDir)
        {
            AtlasFile = atlasFile;
            BuildFile = buildFile;
            OutDir = outDir;
        }

        public TextureFile GetTexturesFile()
        {
            if (convertedTextures) return texturesFile;

            var imageWidth = AtlasFile.Atlas.Width;
            var imageHeight = AtlasFile.Atlas.Height;
            var nameToBitmap = new Dictionary<string, Bitmap>();
            foreach (var symbol in BuildFile.BuildData.Build.SymbolsList)
            foreach (var frame in symbol.FramesList)
            {
                var name = BuildFile.BuildData.HashToName[symbol.Hash] + '_' + frame.SourceFrameNum;
                Console.WriteLine(name);
                if (nameToBitmap.ContainsKey(name))
                {
                    Console.WriteLine($"Warning: symbol {name} was defined more than once!");
                    continue;
                }

                var x1 = (int) (frame.X1 * imageWidth);
                var y1 = (int) (frame.Y1 * imageHeight);
                var width = (int) ((frame.X2 - frame.X1) * imageWidth);
                var height = (int) ((frame.Y2 - frame.Y1) * imageHeight);
                var boundingBox = new Rectangle(x1, y1, width, height);
                var bitmap = AtlasFile.Atlas.Clone(boundingBox, AtlasFile.Atlas.PixelFormat);
                nameToBitmap.Add(name, bitmap);
            }

            texturesFile = new TextureFile(OutDir, nameToBitmap);
            convertedTextures = true;
            return texturesFile;
        }
    }
}