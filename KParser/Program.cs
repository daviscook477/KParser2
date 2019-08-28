using System;
using KParser.Conversion;

namespace KParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Atlas.File atlasFile = new Atlas.Parser("C:\\Users\\Davis\\Documents\\slickster\\oilfloater_0.png").GetFile();
            Build.File buildFile = new Build.Parser("C:\\Users\\Davis\\Documents\\slickster\\oilfloater_build.bytes").GetFile();
            Animation.File animationFile = new Animation.Parser("C:\\Users\\Davis\\Documents\\slickster\\oilfloater_anim.bytes").GetFile();

            KAnimToScmlConverter converter = new KAnimToScmlConverter(atlasFile, buildFile, animationFile);
            Textures.File texturesFile = converter.GetTexturesFile();
            new Textures.Writer("C:\\Users\\Davis\\Documents\\slickster", texturesFile).WriteFile();
            Scml.File scmlFile = converter.GetScmlFile();
            new Scml.Writer("C:\\Users\\Davis\\Documents\\slickster\\door_bunker.scml", scmlFile).WriteFile();
        }
    }
}
