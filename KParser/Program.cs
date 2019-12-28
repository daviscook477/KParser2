﻿using System;
using KParser.Conversion;

namespace KParser
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Atlas.File atlasFile = new Atlas.Parser("C:\\Users\\Davis\\Documents\\kparser2tests\\ac\\airconditioner_0.png").GetFile();
            Build.File buildFile = new Build.Parser("C:\\Users\\Davis\\Documents\\kparser2tests\\ac\\airconditioner_build.bytes").GetFile();
            Animation.File animationFile = new Animation.Parser("C:\\Users\\Davis\\Documents\\kparser2tests\\ac\\airconditioner_anim.bytes").GetFile();

            KAnimToScmlConverter converter = new KAnimToScmlConverter(atlasFile, buildFile, animationFile);
            Textures.File texturesFile = converter.GetTexturesFile();
            new Textures.Writer("C:\\Users\\Davis\\Documents\\kparser2tests\\ac", texturesFile).WriteFile();
            Scml.File scmlFile = converter.GetScmlFile();
            new Scml.Writer("C:\\Users\\Davis\\Documents\\kparser2tests\\ac\\door_bunker.scml", scmlFile).WriteFile();*/

            Scml.File scmlFile = new Scml.Parser("C:\\Users\\Davis\\Documents\\kparser2tests\\ac\\test_interp.scml").GetFile();
            Scml.Conversion.ScmlToAlternateConverter converter = new Scml.Conversion.ScmlToAlternateConverter(scmlFile);
            Scml.Alternate.File alternateFile = converter.GetAlternateFile();
        }
    }
}
