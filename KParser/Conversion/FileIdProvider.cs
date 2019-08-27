using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Conversion
{
    class FileIdProvider
    {
        public Build.File BuildFile { get; internal set; }
        public Dictionary<string, int> NameToFileId { get; internal set; }

        public FileIdProvider(Build.File buildFile)
        {
            BuildFile = buildFile;
            NameToFileId = new Dictionary<string, int>();
            HashSet<string> names = new HashSet<string>();
            int fileId = 0;
            foreach (Build.Symbol symbol in BuildFile.Build.SymbolsList)
            {
                foreach (Build.Frame frame in symbol.FramesList)
                {
                    string name = BuildFile.HashToName[symbol.Hash] + '_' + frame.SourceFrameNum;
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
}
