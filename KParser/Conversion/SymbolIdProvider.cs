using System;
using System.Collections.Generic;
using KParser.Animation;

namespace KParser.Conversion
{
    internal class SymbolIdProvider
    {
        private readonly SymbolOccurenceCache occurenceCache;

        public SymbolIdProvider(Bank bank, Dictionary<int, string> hashToName)
        {
            Bank = bank;
            HashToName = hashToName;
            occurenceCache = new SymbolOccurenceCache(HashToName);
            BuildIdMap();
        }

        public Bank Bank { get; internal set; }
        public Dictionary<int, string> HashToName { get; internal set; }
        public Dictionary<string, int> IdMap { get; internal set; }

        public int GetId(Frame frame, Element element)
        {
            var name = NameOf(frame, element);
            return IdMap[name];
        }

        public string NameOf(Frame frame, Element element)
        {
            var precedingOccurences = occurenceCache.GetPrecedingOccurencesCount(frame, element);
            var name = $"bone_{HashToName[element.Image]}_{precedingOccurences}";
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
            var symbolCountHistogram = GetSymbolCounts();
            IdMap = new Dictionary<string, int>();
            var id = 0;
            foreach (var name in symbolCountHistogram.Keys)
                for (var i = 0; i < symbolCountHistogram[name]; i++)
                {
                    IdMap.Add($"bone_{name}_{i}", id++);
                    Console.WriteLine($"bone_{name}_{i} : {id - 1}");
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
            var maxSymbolCountInAnyFrameHistogram = new Dictionary<string, int>();
            foreach (var frame in Bank.FramesList)
            {
                // determine the maximum number of times a symbol occurs within this specific frame
                var frameSymbolCountHistogram = new Dictionary<string, int>();
                foreach (var element in frame.ElementsList)
                {
                    var name = $"{HashToName[element.Image]}";
                    if (!frameSymbolCountHistogram.ContainsKey(name)) frameSymbolCountHistogram.Add(name, 0);
                    frameSymbolCountHistogram[name]++;
                }

                // update the maximum number of times a symbol
                // occurs within any single frame by comparing against the 
                // previous maximum (stored in the maxSymbolCountInAnyFrameHistogram).
                foreach (var name in frameSymbolCountHistogram.Keys)
                {
                    if (!maxSymbolCountInAnyFrameHistogram.ContainsKey(name))
                        maxSymbolCountInAnyFrameHistogram.Add(name, frameSymbolCountHistogram[name]);
                    maxSymbolCountInAnyFrameHistogram[name] = Math.Max(maxSymbolCountInAnyFrameHistogram[name],
                        frameSymbolCountHistogram[name]);
                }
            }

            return maxSymbolCountInAnyFrameHistogram;
        }
    }
}