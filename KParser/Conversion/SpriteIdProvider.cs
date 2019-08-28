using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Conversion
{
    class SpriteIdProvider
    {
        public Animation.Bank Bank { get; internal set; }
        public Dictionary<int, string> HashToName { get; internal set; }
        public Dictionary<string, int> IdMap { get; internal set; }

        private SpriteOccurenceCache occurenceCache = null;

        public SpriteIdProvider(Animation.Bank bank, Dictionary<int, string> hashToName)
        {
            Bank = bank;
            HashToName = hashToName;
            occurenceCache = new SpriteOccurenceCache(HashToName);
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
            string name = $"{HashToName[element.Image]}_{element.Index}_{precedingOccurences}";
            return name;
        }

        /**
         * Assigns a unique id to each occurence of every sprite in 
         * this specific animation bank.
         * 
         * Spriter references each occurence of a sprite by a unique
         * integer id.
         * 
         * For example if we have two instance of sprite 'b' in an animation
         * each instance of sprite 'b' will have a *different* integer id.
         * 
         * As such, every potential instance of every sprite that exists in
         * this animation bank, is mapped to a unique integer id in this method.
         */
        private void BuildIdMap()
        {
            Dictionary<string, int> elementCountHistogram = GetElementCounts();
            IdMap = new Dictionary<string, int>();
            int id = 0;
            foreach (string name in elementCountHistogram.Keys)
            {
                for (int i = 0; i < elementCountHistogram[name]; i++)
                {
                    IdMap.Add($"{name}_{i}", id++);
                    Console.WriteLine($"{name}_{i} : {id - 1}");
                }
            }
        }

        /**
         * Since a single sprite may be duplicated multiple times
         * within an individual animation bank it is necessary to
         * assign a unique name *not* to the sprite itself but to each
         * occurence of the sprite.
         * 
         * This allows for each instance of that same sprite to be
         * animated separately rather than confused for the same thing.
         * 
         * Unfortunately the Klei animation format does not assign these
         * unique identifiers in the file format. Instead it is expected
         * that if a sprite is duplicated, that many instances of that
         * sprite will exist throughout the animation. This means that
         * the way to identify a sprite is by it's specific image combined
         * with its position in the frame's element list. However it is not
         * the absolute position in the list that can be used since that is
         * allowed to change. Instead it must be identified by it's position
         * with respect to only other instances of the same sprite.
         * 
         * As an example: let the frame element list look like [a, b, c, b]
         * The unique identifiers that will be assigned to these elements
         * are [a_0, b_0, c_0, b_1]
         * In this way if the next frame had an element list of [a, b, b]
         * both instance of sprite 'b' would still be properly identified
         * as 'b_0' and 'b_1' even though their absolute positioning in the list
         * has changed.
         */
        private Dictionary<string, int> GetElementCounts()
        {
            Dictionary<string, int> maxElementCountInAnyFrameHistogram = new Dictionary<string, int>();
            foreach (Animation.Frame frame in Bank.FramesList)
            {
                // determine the maximum number of times a sprite occurs within this specific frame
                Dictionary<string, int> frameElementCountHistogram = new Dictionary<string, int>();
                foreach (Animation.Element element in frame.ElementsList)
                {
                    string name = $"{HashToName[element.Image]}_{element.Index}";
                    if (!frameElementCountHistogram.ContainsKey(name))
                    {
                        frameElementCountHistogram.Add(name, 0);
                    }
                    frameElementCountHistogram[name]++;
                }
                // update the maximum number of times a sprite
                // occurs within any single frame by comparing against the 
                // previous maximum (stored in the maxElementCountInAnyFrameHistogram).
                foreach (string name in frameElementCountHistogram.Keys)
                {
                    if (!maxElementCountInAnyFrameHistogram.ContainsKey(name))
                    {
                        maxElementCountInAnyFrameHistogram.Add(name, frameElementCountHistogram[name]);
                    }
                    maxElementCountInAnyFrameHistogram[name] = Math.Max(maxElementCountInAnyFrameHistogram[name], frameElementCountHistogram[name]);
                }
            }
            return maxElementCountInAnyFrameHistogram;
        }

    }
}
