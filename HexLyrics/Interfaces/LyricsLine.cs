using System;

namespace HexLyrics.Interfaces
{
    public class LyricsLine
    {
        /// <summary>
        /// Start time of this line
        /// </summary>
        public TimeSpan StartTime { get; set; }
        /// <summary>
        /// End time of this line
        /// </summary>
        public TimeSpan StopTime { get; set; }
        /// <summary>
        /// Content of this line
        /// </summary>
        public string Content { get; set; }
    }
}
