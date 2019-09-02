using System;
using System.Collections.Generic;

namespace KParser.Build
{
    internal class Parser
    {
        public const string ExpectedHeader = "BILD";
        public const int MinimumVersion = 10;

        public static BuildData LoadFile(string filePath)
        {
            var buffer = new BinaryReader(System.IO.File.ReadAllBytes(filePath));

            var header = buffer.ReadStringWithLength("header", ExpectedHeader.Length);
            buffer.AdvanceIndex(ExpectedHeader.Length);

            if (!ExpectedHeader.Equals(header))
                throw new InvalidOperationException($"Expected header to be {ExpectedHeader} but found {header}!");

            return new BuildData
            {
                Build = LoadBuild(buffer),
                HashToName = LoadDictionary(buffer)
            };
        }

        private static Build LoadBuild(BinaryReader buffer)
        {
            var version = buffer.ReadInt("version");
            if (version < MinimumVersion)
                throw new InvalidOperationException(
                    $"Expected version to be at least {MinimumVersion} but found {version}!");

            var build = new Build
            {
                Version = version,
                SymbolCount = buffer.ReadInt("symbols"),
                FrameCount = buffer.ReadInt("frames"),
                Name = buffer.ReadString("name"),
                SymbolsList = new List<Symbol>()
            };

            for (var i = 0; i < build.SymbolCount; i++)
            {
                var symbol = new Symbol
                {
                    Hash = buffer.ReadInt("hash"),
                    Path = buffer.ReadInt("path"),
                    Color = buffer.ReadInt("color"),
                    Flags = buffer.ReadInt("flags"),
                    NumFrames = buffer.ReadInt("numFrames"),
                    FramesList = new List<Frame>()
                };

                var time = 0;
                for (var j = 0; j < symbol.NumFrames; j++)
                {
                    var frame = new Frame
                    {
                        SourceFrameNum = buffer.ReadInt("sourceFrameNum"),
                        Duration = buffer.ReadInt("duration"),
                        BuildImageIdx = buffer.ReadInt("buildImageIdx"),
                        PivotX = buffer.ReadFloat("pivotX"),
                        PivotY = buffer.ReadFloat("pivotY"),
                        PivotWidth = buffer.ReadFloat("pivotWidth"),
                        PivotHeight = buffer.ReadFloat("pivotHeight"),
                        X1 = buffer.ReadFloat("x1"),
                        Y1 = buffer.ReadFloat("y1"),
                        X2 = buffer.ReadFloat("x2"),
                        Y2 = buffer.ReadFloat("y2")
                    };
                    time += frame.Duration;
                    symbol.FramesList.Add(frame);
                }

                build.SymbolsList.Add(symbol);
            }

            return build;
        }

        private static Dictionary<int, string> LoadDictionary(BinaryReader buffer)
        {
            var hashToName = new Dictionary<int, string>();
            var numHashes = buffer.ReadInt("numHashes");
            for (var i = 0; i < numHashes; i++)
            {
                var hash = buffer.ReadInt("hash");
                var name = buffer.ReadString("name");
                hashToName.Add(hash, name);
            }

            return hashToName;
        }
    }
}