using System.Collections.Generic;
using KParser.Animation;

namespace KParser.Conversion
{
    internal class SymbolOccurenceCache
    {
        private Frame associatedFrame;

        private Dictionary<Element, int> occurenceMap;

        public SymbolOccurenceCache(Dictionary<int, string> hashToName)
        {
            HashToName = hashToName;
        }

        public Dictionary<int, string> HashToName { get; internal set; }

        /**
         * Gets the number of times the symbol used by this element was used by elements
         * in the given frame that came before this element in the frame's element list.
         * 
         * Uses a simple caching scheme based on the frame for performance improvements.
         */
        public int GetPrecedingOccurencesCount(Frame frame, Element element)
        {
            if (frame != associatedFrame) RebuildCache(frame);

            return occurenceMap[element];
        }

        private void RebuildCache(Frame frame)
        {
            occurenceMap = new Dictionary<Element, int>();
            associatedFrame = frame;
            var rollingOccurenceCount = new Dictionary<string, int>();
            foreach (var element in frame.ElementsList)
            {
                var name = $"{HashToName[element.Image]}";
                if (!rollingOccurenceCount.ContainsKey(name)) rollingOccurenceCount.Add(name, 0);
                occurenceMap.Add(element, rollingOccurenceCount[name]++);
            }
        }
    }
}