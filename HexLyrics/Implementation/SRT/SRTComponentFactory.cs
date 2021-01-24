using System;
using System.Collections.Generic;
using System.Text;
using HexLyrics.Interfaces;

namespace HexLyrics.Implementation.SRT
{
    [LyricsComponent("SRT")]
    class SRTComponentFactory : ILyricsComponentFactory
    {
        public ILyricsFormatter GenerateFormatter(LyricsContent content)
            => new SRTFormatter(content);

        public ILyricsParser GenerateParser(string content)
            => throw new NotImplementedException("SRT parser not available now.");
    }
}
