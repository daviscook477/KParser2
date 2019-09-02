﻿using System;
using System.Text;

namespace KParser
{
    internal class BinaryReader
    {
        private int index;

        public BinaryReader(byte[] rawData)
        {
            RawData = rawData;
        }

        public byte[] RawData { get; internal set; }

        public int ReadInt(string name)
        {
            const int size = 4;
            if (index + size > RawData.Length)
                throw new InvalidOperationException(
                    $"Attempted to read {name} as an integer (4 bytes) at index {index} but this exceeds the size of the byte buffer ({RawData.Length})!");
            var value = BitConverter.ToInt32(Subset(index, 4, true));
            index += size;
            return value;
        }

        public float ReadFloat(string name)
        {
            const int size = 4;
            if (index + size > RawData.Length)
                throw new InvalidOperationException(
                    $"Attempted to read {name} as a float (4 bytes) at index {index} but this exceeds the size of the byte buffer ({RawData.Length})!");
            var value = BitConverter.ToSingle(Subset(index, 4, true));
            index += size;
            return value;
        }

        public string ReadString(string name)
        {
            var length = ReadInt($"length of {name}");
            if (length < 0)
                throw new InvalidOperationException(
                    $"Attempted to read {name} as a string (indeterminate bytes) at index {index} but reading the length of {name} failed with length < 0!");
            var value = ReadStringWithLength(name, length);
            index += length;
            return value;
        }

        public string ReadStringWithLength(string name, int length)
        {
            if (index + length > RawData.Length)
                throw new InvalidOperationException(
                    $"Attempted to read {name} as a string ({length} bytes) at index {index} but this exceeds the size of the byte buffer ({RawData.Length})!");
            return Encoding.UTF8.GetString(Subset(index, length, false));
        }

        public void AdvanceIndex(int amount)
        {
            index += amount;
        }

        private byte[] Subset(int index, int length, bool respectEndianness)
        {
            var subset = new byte[length];
            Array.Copy(RawData, index, subset, 0, length);
            if (respectEndianness && ShouldReverseBytes()) Array.Reverse(subset);
            return subset;
        }

        private bool ShouldReverseBytes()
        {
            return !BitConverter.IsLittleEndian;
        }
    }
}