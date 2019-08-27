using System.Collections.Generic;

namespace KParser.Animation
{
    class Bank
    {
        public string Name { get; set; }
        public int Hash { get; set; }
        public float Rate { get; set; }
        public int Frames { get; set; }
        public List<Frame> FramesList { get; set; }

        public Bank(string name, int hash, float rate, int frames, List<Frame> framesList)
        {
            Name = name;
            Hash = hash;
            Rate = rate;
            Frames = frames;
            FramesList = framesList;
        }
    }
}
