using System.Collections.Generic;

namespace HexLyrics.Interfaces
{
    public class LyricsContent
    {
        /// <summary>
        /// Song title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The author of the song
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Singer of the song
        /// </summary>
        public string Singer { get; set; }
        /// <summary>
        /// Album the song belongs to
        /// </summary>
        public string Album { get; set; }
        /// <summary>
        /// The creator of the song
        /// </summary>
        public string LyricsCreator { get; set; }
        /// <summary>
        /// Lyrics lines of the song
        /// </summary>
        public LyricsLine[] Lines { get; set; }
        /// <summary>
        /// Context of the song, representing the additional information that may vary between different format
        /// </summary>
        public Dictionary<string, string> Context { get; set; }
    }
}
