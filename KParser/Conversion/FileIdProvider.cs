using System;
using System.Collections.Generic;
using AnimData;

namespace KParser.Conversion
{
    internal class FileIdProvider
    {
        public BuildFile BuildFile;
        public Dictionary<string, int> NameToFileId;

        public FileIdProvider(BuildFile buildFile)
        {
            BuildFile = buildFile;
            NameToFileId = new Dictionary<string, int>();
            var names = new HashSet<string>();
            var fileId = 0;
            foreach (var symbol in BuildFile.BuildData.Build.SymbolsList)
            foreach (var frame in symbol.FramesList)
            {
                var name = BuildFile.BuildData.HashToName[symbol.Hash] + '_' + frame.SourceFrameNum;
                if (names.Contains(name))
                {
                    Console.WriteLine($"Warning: symbol {name} was defined more than once!");
                    continue;
                }

                names.Add(name);
                NameToFileId.Add(name, fileId++);
            }
        }
    }
}