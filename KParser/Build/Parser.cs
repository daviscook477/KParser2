using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Build
{
    class Parser
    {
        public const string ExpectedHeader = "BILD";
        public const int MinimumVersion = 10;

        public string Path { get; internal set; }

        private bool loaded = false;
        private File file = null;

        public Parser(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                throw new ArgumentException($"The build file specified at {path} does not exist!");
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
            if(!ExpectedHeader.Equals(header))
            {
                throw new InvalidOperationException($"Expected header to be {ExpectedHeader} but found {header}!");
            }

            Build build = LoadBuild(buffer);
            Dictionary<int, string> hashToName = LoadDictionary(buffer);
            file = new File(build, hashToName);
        }

        private Build LoadBuild(BinaryReader buffer)
        {
            int version = buffer.ReadInt("version");
            if (version < MinimumVersion)
            {
                throw new InvalidOperationException($"Expected version to be at least {MinimumVersion} but found {version}!");
            }

            int symbols = buffer.ReadInt("symbols");
            int frames = buffer.ReadInt("frames");
            string name = buffer.ReadString("name");
            List<Symbol> symbolsList = new List<Symbol>();
            Build build = new Build(version, symbols, frames, name, symbolsList);

            for (int i = 0; i < build.Symbols; i++)
            {
                int hash = buffer.ReadInt("hash");
                int path = buffer.ReadInt("path");
                int color = buffer.ReadInt("color");
                int flags = buffer.ReadInt("flags");
                int numFrames = buffer.ReadInt("numFrames");
                List<Frame> framesList = new List<Frame>();
                Symbol symbol = new Symbol(hash, path, color, flags, numFrames, framesList);

                int time = 0;
                for (int j = 0; j < symbol.NumFrames; j++)
                {
                    int sourceFrameNum = buffer.ReadInt("sourceFrameNum");
                    int duration = buffer.ReadInt("duration");
                    int buildImageIdx = buffer.ReadInt("buildImageIdx");
                    float pivotX = buffer.ReadFloat("pivotX");
                    float pivotY = buffer.ReadFloat("pivotY");
                    float pivotWidth = buffer.ReadFloat("pivotWidth");
                    float pivotHeight = buffer.ReadFloat("pivotHeight");
                    float x1 = buffer.ReadFloat("x1");
                    float y1 = buffer.ReadFloat("y1");
                    float x2 = buffer.ReadFloat("x2");
                    float y2 = buffer.ReadFloat("y2");
                    Frame frame = new Frame(sourceFrameNum, duration, buildImageIdx, pivotX, pivotY, pivotWidth, pivotHeight, x1, y1, x2, y2, time);
                    time += frame.Duration;
                    symbol.FramesList.Add(frame);
                }
                build.SymbolsList.Add(symbol);
            }

            return build;
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
