using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Animation
{
    class Parser
    {
        public const string ExpectedHeader = "ANIM";

        public string Path { get; internal set; }

        private bool loaded = false;
        private File file = null;

        public Parser(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                throw new ArgumentException($"The animation file specified at {path} does not exist!");
            }
            Path = path;
        }

        public File GetFile()
        {
            if (!loaded)
            {
                LoadFile();
                loaded = true;
            }
            return file;
        }

        private void LoadFile()
        {
            byte[] bytes = System.IO.File.ReadAllBytes(Path);
            BinaryReader buffer = new BinaryReader(bytes);

            string header = buffer.ReadStringWithLength("header", ExpectedHeader.Length);
            buffer.AdvanceIndex(ExpectedHeader.Length);
            if (!ExpectedHeader.Equals(header))
            {
                throw new InvalidOperationException($"Expected header to be {ExpectedHeader} but found {header}!");
            }

            Animation animation = LoadAnimation(buffer);
            Dictionary<int, string> hashToName = LoadDictionary(buffer);
            file = new File(animation, hashToName);
        }

        private Animation LoadAnimation(BinaryReader buffer)
        {
            int version = buffer.ReadInt("version");
            int elements = buffer.ReadInt("elements");
            int frames = buffer.ReadInt("frames");
            int animations = buffer.ReadInt("animations");
            List<Bank> banksList = new List<Bank>();
            Animation animation = new Animation(version, elements, frames, animations, banksList, -1);

            for (int i = 0; i < animation.Animations; i++)
            {
                string name = buffer.ReadString("name");
                int hash = buffer.ReadInt("hash");
                float rate = buffer.ReadFloat("rate");
                int frames1 = buffer.ReadInt("frames");
                List<Frame> framesList = new List<Frame>();
                Bank bank = new Bank(name, hash, rate, frames1, framesList);

                for (int j = 0; j < bank.Frames; j++)
                {
                    float x = buffer.ReadFloat("x");
                    float y = buffer.ReadFloat("y");
                    float width = buffer.ReadFloat("width");
                    float height = buffer.ReadFloat("height");
                    int elements1 = buffer.ReadInt("elements");
                    List<Element> elementsList = new List<Element>();
                    Frame frame = new Frame(x, y, width, height, elements1, elementsList);

                    for (int k = 0; k < frame.Elements; k++)
                    {
                        int image = buffer.ReadInt("image");
                        int index = buffer.ReadInt("index");
                        int layer = buffer.ReadInt("layer");
                        int flags = buffer.ReadInt("flags");
                        float a = buffer.ReadFloat("a");
                        float b = buffer.ReadFloat("b");
                        float g = buffer.ReadFloat("g");
                        float r = buffer.ReadFloat("r");
                        float m1 = buffer.ReadFloat("m1");
                        float m2 = buffer.ReadFloat("m2");
                        float m3 = buffer.ReadFloat("m3");
                        float m4 = buffer.ReadFloat("m4");
                        float m5 = buffer.ReadFloat("m5");
                        float m6 = buffer.ReadFloat("m6");
                        float order = buffer.ReadFloat("order");
                        Element element = new Element(image, index, layer, flags, a, b, g, r, m1, m2, m3, m4, m5, m6, order);
                        frame.ElementsList.Add(element);
                    }
                    bank.FramesList.Add(frame);
                }
                animation.BanksList.Add(bank);
            }
            animation.MaxVisSymbolFrames = buffer.ReadInt("maxVisSymbolFrames");
            return animation;
        }

        private Dictionary<int, string> LoadDictionary(BinaryReader buffer)
        {
            Dictionary<int, string> hashToName = new Dictionary<int, string>();
            int numHashes = buffer.ReadInt("numHashes");
            for (int i = 0; i < numHashes; i++)
            {
                int hash = buffer.ReadInt("hash");
                string name = buffer.ReadString("name");
                hashToName.Add(hash, name);
            }
            return hashToName;
        }
    }
}
