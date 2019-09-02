using System;
using KParser.Build;

namespace KParser.File
{
    internal class BuildFile : IFile
    {
        public const string Header = "BILD";
        public BuildData BuildData;

        public string FilePath;

        //private bool written = false;

        public BuildFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                Console.WriteLine($"Warning: the file at {filePath} already exists! Overwriting...");
            FilePath = filePath;
            BuildData = Parser.LoadFile(filePath);
        }

        public bool WriteFile()
        {
            var buffer = new BinaryWriter(FilePath);

            buffer.WriteRawString(Header);
            buffer.WriteInt(BuildData.Build.Version);
            buffer.WriteInt(BuildData.Build.SymbolCount);
            buffer.WriteInt(BuildData.Build.FrameCount);
            buffer.WriteString(BuildData.Build.Name);

            foreach (var symbol in BuildData.Build.SymbolsList)
            {
                buffer.WriteInt(symbol.Hash);
                buffer.WriteInt(symbol.Path);
                buffer.WriteInt(symbol.Color);
                buffer.WriteInt(symbol.Flags);
                buffer.WriteInt(symbol.NumFrames);

                foreach (var frame in symbol.FramesList)
                {
                    buffer.WriteInt(frame.SourceFrameNum);
                    buffer.WriteInt(frame.Duration);
                    buffer.WriteInt(frame.BuildImageIdx);
                    buffer.WriteFloat(frame.PivotX);
                    buffer.WriteFloat(frame.PivotY);
                    buffer.WriteFloat(frame.PivotWidth);
                    buffer.WriteFloat(frame.PivotHeight);
                    buffer.WriteFloat(frame.X1);
                    buffer.WriteFloat(frame.Y1);
                    buffer.WriteFloat(frame.X2);
                    buffer.WriteFloat(frame.Y2);
                }
            }

            buffer.WriteInt(BuildData.HashToName.Count);
            foreach (var (hash, name) in BuildData.HashToName)
            {
                buffer.WriteInt(hash);
                buffer.WriteString(name);
            }

            return true;
        }
    }
}