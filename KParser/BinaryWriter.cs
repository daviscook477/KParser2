using System;
using System.Collections.Generic;
using System.Text;

namespace KParser
{
    internal class BinaryWriter
    {
        public BinaryWriter(string path)
        {
            Path = path;
            RawData = new List<byte>();
        }

        public string Path { get; internal set; }
        public List<byte> RawData { get; internal set; }

        public void Flush()
        {
            System.IO.File.WriteAllBytes(Path, RawData.ToArray());
        }

        public void WriteInt(int value)
        {
            RawData.AddRange(BitConverter.GetBytes(value));
        }

        public void WriteFloat(float value)
        {
            RawData.AddRange(BitConverter.GetBytes(value));
        }

        public void WriteString(string value)
        {
            WriteInt(value.Length);
            WriteRawString(value);
        }

        public void WriteRawString(string value)
        {
            RawData.AddRange(Encoding.UTF8.GetBytes(value));
        }
    }
}