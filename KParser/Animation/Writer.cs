using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Animation
{
    class Writer
    {
        public const string Header = "ANIM";

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
            buffer.WriteInt(File.Animation.Version);
            buffer.WriteInt(File.Animation.Elements);
            buffer.WriteInt(File.Animation.Frames);
            buffer.WriteInt(File.Animation.Animations);

            foreach (Bank bank in File.Animation.BanksList)
            {
                buffer.WriteString(bank.Name);
                buffer.WriteInt(bank.Hash);
                buffer.WriteFloat(bank.Rate);
                buffer.WriteInt(bank.Frames);

                foreach (Frame frame in bank.FramesList)
                {
                    buffer.WriteFloat(frame.X);
                    buffer.WriteFloat(frame.Y);
                    buffer.WriteFloat(frame.Width);
                    buffer.WriteFloat(frame.Height);
                    buffer.WriteInt(frame.Elements);

                    foreach (Element element in frame.ElementsList)
                    {
                        buffer.WriteInt(element.Image);
                        buffer.WriteInt(element.Index);
                        buffer.WriteInt(element.Layer);
                        buffer.WriteInt(element.Flags);
                        buffer.WriteFloat(element.A);
                        buffer.WriteFloat(element.B);
                        buffer.WriteFloat(element.G);
                        buffer.WriteFloat(element.R);
                        buffer.WriteFloat(element.M1);
                        buffer.WriteFloat(element.M2);
                        buffer.WriteFloat(element.M3);
                        buffer.WriteFloat(element.M4);
                        buffer.WriteFloat(element.M5);
                        buffer.WriteFloat(element.M6);
                        buffer.WriteFloat(element.Order);
                    }
                }
            }
            buffer.WriteInt(File.Animation.MaxVisSymbolFrames);

            buffer.WriteInt(File.HashToName.Count);
            foreach (KeyValuePair<int, string> entry in File.HashToName)
            {
                buffer.WriteInt(entry.Key);
                buffer.WriteString(entry.Value);
            }
        }
    }
}
