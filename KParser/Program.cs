using System.Collections.Generic;
using AnimData;
using KParser.Conversion;
using KParser.File;

namespace KParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var animFiles = new List<AnimFiles>
            {
                new AnimFiles
                {
                    Atlas = new AtlasFile("../../../TestAnims/oilfloater_0.png"),
                    Build = new BuildFile("../../../TestAnims/oilfloater_build.bytes"),
                    Anim = new AnimFile("../../../TestAnims/oilfloater_anim.bytes")
                }
            };

            foreach (var anim in animFiles)
            {
                var converter = new KAnimToScmlConverter(anim.Atlas, anim.Build, anim.Anim, "../../../TestAnims/out/");
                converter.GetTexturesFile().WriteFile();
                converter.GetScmlFile().WriteFile();
            }
        }
    }

    internal struct AnimFiles
    {
        public AtlasFile Atlas;
        public BuildFile Build;
        public AnimFile Anim;
    }
}