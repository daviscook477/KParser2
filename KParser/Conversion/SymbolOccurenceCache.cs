using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Conversion
{
    class SymbolOccurenceCache
    {
        public Dictionary<int, string> HashToName { get; internal set; }

        public SymbolOccurenceCache(Dictionary<int, string> hashToName)
        {
            HashToName = hashToName;
        }

        private Dictionary<Animation.Element, int> occurenceMap = null;
        private Animation.Frame associatedFrame = null;

        /**
         * Gets the number of times the symbol used by this element was used by elements
         * in the given frame that came before this element in the frame's element list.
         * 
         * Uses a simple caching scheme based on the frame for performance improvements.
         */
        public int GetPrecedingOccurencesCount(Animation.Frame frame, Animation.Element element)
        {
            if (frame != associatedFrame)
            {
                RebuildCache(frame);
            }

            return occurenceMap[element];
        }

        private void RebuildCache(Animation.Frame frame)
        {
            occurenceMap = new Dictionary<Animation.Element, int>();
            associatedFrame = frame;
            Dictionary<string, int> rollingOccurenceCount = new Dictionary<string, int>();
            foreach (Animation.Element element in frame.ElementsList)
            {
                string name = $"{HashToName[element.Image]}";
                if (!rollingOccurenceCount.ContainsKey(name))
                {
                    rollingOccurenceCount.Add(name, 0);
                }
                occurenceMap.Add(element, rollingOccurenceCount[name]++);
            }
        }
    }
}
