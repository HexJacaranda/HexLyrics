using HexLyrics.Interfaces;
using System;

namespace HexLyrics.Implementation.LRC
{
    [LyricsComponent("LRC")]
    class LRCComponentFactory : ILyricsComponentFactory
    {
        public ILyricsFormatter GenerateFormatter(LyricsContent content)
            => new LRCFormatter(content);

        public ILyricsParser GenerateParser(string content)
            => new LRCParser(content);
    }
}
