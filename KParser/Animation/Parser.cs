using System;
using System.Collections.Generic;

namespace KParser.Animation
{
    internal class Parser
    {
        public const string ExpectedHeader = "ANIM";

        public static AnimData LoadFile(string filePath)
        {
            var bytes = System.IO.File.ReadAllBytes(filePath);
            var buffer = new BinaryReader(bytes);

            var header = buffer.ReadStringWithLength("header", ExpectedHeader.Length);
            buffer.AdvanceIndex(ExpectedHeader.Length);
            if (!ExpectedHeader.Equals(header))
                throw new InvalidOperationException($"Expected header to be {ExpectedHeader} but found {header}!");

            return new AnimData
            {
                Animation = LoadAnimation(buffer),
                HashToName = LoadDictionary(buffer)
            };
        }

        private static Animation LoadAnimation(BinaryReader buffer)
        {
            var animation = new Animation
            {
                Version = buffer.ReadInt("version"),
                ElementCount = buffer.ReadInt("elements"),
                FrameCount = buffer.ReadInt("frames"),
                AnimationCount = buffer.ReadInt("animations"),
                BanksList = new List<Bank>(),
                MaxVisSymbolFrames = -1
            };

            for (var i = 0; i < animation.AnimationCount; i++)
            {
                var bank = new Bank
                {
                    Name = buffer.ReadString("name"),
                    Hash = buffer.ReadInt("hash"),
                    Rate = buffer.ReadFloat("rate"),
                    FrameCount = buffer.ReadInt("frames"),
                    FramesList = new List<Frame>()
                };

                for (var j = 0; j < bank.FrameCount; j++)
                {
                    var frame = new Frame
                    {
                        X = buffer.ReadFloat("x"),
                        Y = buffer.ReadFloat("y"),
                        Width = buffer.ReadFloat("width"),
                        Height = buffer.ReadFloat("height"),
                        ElementCount = buffer.ReadInt("elements"),
                        ElementsList = new List<Element>()
                    };

                    for (var k = 0; k < frame.ElementCount; k++)
                    {
                        var element = new Element
                        {
                            Image = buffer.ReadInt("image"),
                            Index = buffer.ReadInt("index"),
                            Layer = buffer.ReadInt("layer"),
                            Flags = buffer.ReadInt("flags"),
                            R = buffer.ReadFloat("r"),
                            G = buffer.ReadFloat("g"),
                            B = buffer.ReadFloat("b"),
                            A = buffer.ReadFloat("a"),
                            M1 = buffer.ReadFloat("m1"),
                            M2 = buffer.ReadFloat("m2"),
                            M3 = buffer.ReadFloat("m3"),
                            M4 = buffer.ReadFloat("m4"),
                            M5 = buffer.ReadFloat("m5"),
                            M6 = buffer.ReadFloat("m6"),
                            ZIndex = -1
                        };
                        frame.ElementsList.Add(element);
                    }

                    bank.FramesList.Add(frame);
                }

                animation.BanksList.Add(bank);
            }

            animation.MaxVisSymbolFrames = buffer.ReadInt("maxVisSymbolFrames");
            return animation;
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