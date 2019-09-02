using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using KParser.Conversion;
using KParser.File;

namespace KParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            
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

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            Console.WriteLine($"Runtime: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}");
        }
    }

    internal struct AnimFiles
    {
        public AtlasFile Atlas;
        public BuildFile Build;
        public AnimFile Anim;
    }
}