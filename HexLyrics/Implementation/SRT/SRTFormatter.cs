using System;
using System.Text;
using HexLyrics.Interfaces;

namespace HexLyrics.Implementation.SRT
{
    [LyricsComponent("SRT")]
    class SRTFormatter : ILyricsFormatter
    {
        private readonly LyricsContent mContent;
        private readonly StringBuilder mBuilder = new StringBuilder();
        public SRTFormatter(LyricsContent content)
        {
            mContent = content;
        }
        
        public string Format()
        {
            int lineCount = 1;
            foreach (var line in mContent.Lines)
            {
                mBuilder.AppendLine(lineCount.ToString());
                TimeSpan from = line.StartTime;
                TimeSpan to = line.StopTime;
                mBuilder.AppendLine($"{from.Hours:D2}:{from.Minutes:D2}:{from.Seconds:D2},{from.Milliseconds:D3} --> {to.Hours:D2}:{to.Minutes:D2}:{to.Seconds:D2},{to.Milliseconds:D3}");
                mBuilder.AppendLine(line.Content);
                mBuilder.AppendLine();
                lineCount++;
            }
            return mBuilder.ToString();
        }
    }
}
