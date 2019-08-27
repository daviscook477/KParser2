using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Build
{
    class Writer
    {
        public const string Header = "BILD";

        public string Path { get; internal set; }
        public File File { get; internal set; }

        private bool written = false;

        public Writer(string path, File file)
        {
            Path = path;
            if (System.IO.File.Exists(path))
            {
                Console.WriteLine($"Warning: the file at {path} already exists! Overwriting {path}...");
            }

            File = file;
        }

        public void WriteFile()
        {
            if (!written)
            {
                WriteFileInternal();
                written = true;
            }
        }

        private void WriteFileInternal()
        {
            BinaryWriter buffer = new BinaryWriter(Path);

            buffer.WriteRawString(Header);
            buffer.WriteInt(File.Build.Version);
            buffer.WriteInt(File.Build.Symbols);
            buffer.WriteInt(File.Build.Frames);
            buffer.WriteString(File.Build.Name);

            foreach (Symbol symbol in File.Build.SymbolsList)
            {
                buffer.WriteInt(symbol.Hash);
                buffer.WriteInt(symbol.Path);
                buffer.WriteInt(symbol.Color);
                buffer.WriteInt(symbol.Flags);
                buffer.WriteInt(symbol.NumFrames);

                foreach (Frame frame in symbol.FramesList)
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

            buffer.WriteInt(File.HashToName.Count);
            foreach (KeyValuePair<int, string> entry in File.HashToName)
            {
                buffer.WriteInt(entry.Key);
                buffer.WriteString(entry.Value);
            }
        }
    }
}
