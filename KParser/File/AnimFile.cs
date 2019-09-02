using System;
using System.IO;
using KParser.Animation;

namespace KParser.File
{
    internal class AnimFile : IFile
    {
        public const string Header = "ANIM";
        public Animation.AnimData AnimData;

        public string FilePath;

        public AnimFile(string filePath)
        {
            FilePath = filePath;
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException($"The anim file at {filePath} does not exist!");
            }

            AnimData = Parser.LoadFile(filePath);
        }

        public bool WriteFile()
        {
            var buffer = new BinaryWriter(FilePath);

            buffer.WriteRawString(Header);
            buffer.WriteInt(AnimData.Animation.Version);
            buffer.WriteInt(AnimData.Animation.ElementCount);
            buffer.WriteInt(AnimData.Animation.FrameCount);
            buffer.WriteInt(AnimData.Animation.AnimationCount);

            foreach (var bank in AnimData.Animation.BanksList)
            {
                buffer.WriteString(bank.Name);
                buffer.WriteInt(bank.Hash);
                buffer.WriteFloat(bank.Rate);
                buffer.WriteInt(bank.FrameCount);

                foreach (var frame in bank.FramesList)
                {
                    buffer.WriteFloat(frame.X);
                    buffer.WriteFloat(frame.Y);
                    buffer.WriteFloat(frame.Width);
                    buffer.WriteFloat(frame.Height);
                    buffer.WriteInt(frame.ElementCount);

                    foreach (var element in frame.ElementsList)
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

            buffer.WriteInt(AnimData.Animation.MaxVisSymbolFrames);

            buffer.WriteInt(AnimData.HashToName.Count);
            foreach (var (hash, name) in AnimData.HashToName)
            {
                buffer.WriteInt(hash);
                buffer.WriteString(name);
            }

            return true;
        }
    }
}