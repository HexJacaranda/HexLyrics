using System;
using System.Collections.Generic;
using System.Text;
using HexLyrics.Interfaces;

namespace HexLyrics.Implementation.LRC
{
    [LyricsComponent("LRC")]
    class LRCFormatter : ILyricsFormatter
    {
        private readonly LyricsContent mContent;
        public LRCFormatter(LyricsContent content)
        {
            mContent = content;
        }
        public string Format()
        {

            return string.Empty;
        }
    }
}
