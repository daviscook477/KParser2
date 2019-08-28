using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Conversion
{
    class SymbolIdProvider
    {
        public Animation.Bank Bank { get; internal set; }
        public int Offset { get; internal set; }
        public Dictionary<int, string> HashToName { get; internal set; }
        public Dictionary<string, int> IdMap { get; internal set; }

        private SymbolOccurenceCache occurenceCache = null;

        public SymbolIdProvider(Animation.Bank bank, int offset, Dictionary<int, string> hashToName)
        {
            Bank = bank;
            Offset = offset;
            HashToName = hashToName;
            occurenceCache = new SymbolOccurenceCache(HashToName);
            BuildIdMap();
        }

        public int GetId(Animation.Frame frame, Animation.Element element)
        {
            string name = NameOf(frame, element);
            return IdMap[name];
        }

        public string NameOf(Animation.Frame frame, Animation.Element element)
        {
            int precedingOccurences = occurenceCache.GetPrecedingOccurencesCount(frame, element);
            string name = $"bone_{HashToName[element.Image]}_{precedingOccurences}";
            return name;
        }

        /**
         * Assigns a unique id to each occurence of every symbol in
         * this specific animation bank.
         * 
         * Spriter references each bone by a unique integer id.
         * 
         * For example if we have two instances of symbol 'b' in an animation
         * each instance of symbol 'b' will have a *different* integer id.
         * 
         * As such, every potential instance of every symbol that exists in
         * this animation bank, is mapped to a unique integer id in this method.
         */
        private void BuildIdMap()
        {
            Dictionary<string, int> symbolCountHistogram = GetSymbolCounts();
            IdMap = new Dictionary<string, int>();
            int id = Offset;
            foreach (string name in symbolCountHistogram.Keys)
            {
                for (int i = 0; i < symbolCountHistogram[name]; i++)
                {
                    IdMap.Add($"bone_{name}_{i}", id++);
                    Console.WriteLine($"bone_{name}_{i} : {id - 1}");
                }
            }
        }

        /**
         * Since a single symbol may be duplicated multiple times
         * within an individual animation bank it is necessary to
         * assign a unique name *not* to the symbol itself but to each
         * occurence of the symbol.
         * 
         * This allows for each instance of that same symbol to be
         * animated separately rather than confused for the same thing.
         */
         private Dictionary<string, int> GetSymbolCounts()
        {
            Dictionary<string, int> maxSymbolCountInAnyFrameHistogram = new Dictionary<string, int>();
            foreach (Animation.Frame frame in Bank.FramesList)
            {
                // determine the maximum number of times a symbol occurs within this specific frame
                Dictionary<string, int> frameSymbolCountHistogram = new Dictionary<string, int>();
                foreach (Animation.Element element in frame.ElementsList)
                {
                    string name = $"{HashToName[element.Image]}";
                    if (!frameSymbolCountHistogram.ContainsKey(name))
                    {
                        frameSymbolCountHistogram.Add(name, 0);
                    }
                    frameSymbolCountHistogram[name]++;
                }
                // update the maximum number of times a symbol
                // occurs within any single frame by comparing against the 
                // previous maximum (stored in the maxSymbolCountInAnyFrameHistogram).
                foreach (string name in frameSymbolCountHistogram.Keys)
                {
                    if (!maxSymbolCountInAnyFrameHistogram.ContainsKey(name))
                    {
                        maxSymbolCountInAnyFrameHistogram.Add(name, frameSymbolCountHistogram[name]);
                    }
                    maxSymbolCountInAnyFrameHistogram[name] = Math.Max(maxSymbolCountInAnyFrameHistogram[name], frameSymbolCountHistogram[name]);
                }
            }
            return maxSymbolCountInAnyFrameHistogram;
        }
    }
}
