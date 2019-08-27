using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Build
{
    class Build
    {
        public int Version { get; set; }
        public int Symbols { get; set; }
        public int Frames { get; set; }
        public string Name { get; set; }
        public List<Symbol> SymbolsList { get; set; }

        public Build(int version, int symbols, int frames, string name, List<Symbol> symbolsList)
        {
            Version = version;
            Symbols = symbols;
            Frames = frames;
            Name = name;
            SymbolsList = symbolsList;
        }
    }
}
