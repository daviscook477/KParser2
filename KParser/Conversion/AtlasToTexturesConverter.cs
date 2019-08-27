using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace KParser.Conversion
{
    class AtlasToTexturesConverter
    {
        public Atlas.File AtlasFile { get; internal set; }
        public Build.File BuildFile { get; internal set; }

        private bool converted = false;
        private Textures.File texturesFile = null;

        public AtlasToTexturesConverter(Atlas.File atlasFile, Build.File buildFile)
        {
            AtlasFile = atlasFile;
            BuildFile = buildFile;
        }

        public Textures.File GetTexturesFile()
        {
            if (!converted)
            {
                ConvertFile();
                converted = true;
            }
            return texturesFile;
        }

        private void ConvertFile()
        {
            int imageWidth = AtlasFile.Atlas.Width;
            int imageHeight = AtlasFile.Atlas.Height;
            Dictionary<string, Bitmap> nameToBitmap = new Dictionary<string, Bitmap>();
            foreach (Build.Symbol symbol  in BuildFile.Build.SymbolsList)
            {
                foreach (Build.Frame frame in symbol.FramesList)
                {
                    string name = BuildFile.HashToName[symbol.Hash] + '_' + frame.SourceFrameNum;
                    Console.WriteLine(name);
                    if (nameToBitmap.ContainsKey(name))
                    {
                        Console.WriteLine($"Warning: symbol {name} was defined more than once!");
                        continue;
                    }

                    int x1 = (int) (frame.X1 * imageWidth);
                    int y1 = (int) ((frame.Y1 ) * imageHeight);
                    int width = (int) ((frame.X2 - frame.X1) * imageWidth);
                    int height = (int) ((frame.Y2 - frame.Y1) * imageHeight);
                    Rectangle boundingBox = new Rectangle(x1, y1, width, height);
                    Bitmap bitmap = AtlasFile.Atlas.Clone(boundingBox, AtlasFile.Atlas.PixelFormat);
                    nameToBitmap.Add(name, bitmap);
                }
            }
            texturesFile = new Textures.File(nameToBitmap);
        }
    }
}
