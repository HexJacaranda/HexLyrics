using HexLyrics.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HexLyrics.Implementation.LRC
{
    /// <summary>
    /// LRC Parser
    /// </summary>
    [LyricsComponent("LRC")]
    class LRCParser : ILyricsParser
    {
        private readonly string mValue;
        private int mIndex = 0;
        private readonly int mMessageLookAroundOffset = 16;
        private readonly List<(TimeSpan Time, string Content)> mLines = new List<(TimeSpan Time, string Content)>();
        private readonly LyricsContent mContent = new LyricsContent();
        private readonly Dictionary<string, Action<string>> mPropertyInput;
        public LRCParser(string value)
        {
            mValue = value;
            mPropertyInput = new Dictionary<string, Action<string>>
            {
                { "au", x => mContent.Author = x },
                { "ar", x => mContent.Singer = x },
                { "al", x => mContent.Album = x },
                { "by", x => mContent.LyricsCreator = x },
                { "ti", x => mContent.Title = x }
            };
        }
        /// <summary>
        /// The exception message (contains lyrics context) to show when coming to the error
        /// </summary>
        private string IllegalContentMessage { 
            get
            {
                int leftIndex = mIndex - mMessageLookAroundOffset;
                if (leftIndex < 0) leftIndex = 0;
                int rightIndex = mIndex + mMessageLookAroundOffset;
                if (rightIndex > mValue.Length) rightIndex = mValue.Length;
                return $"Context: {mValue[leftIndex..rightIndex]}";
            } 
        }
        /// <summary>
        /// Check and read
        /// </summary>
        private char NowCharChecked
        {
            get
            {
                if (mIndex >= mValue.Length)
                    ThrowIfUnreadableNow(IllegalContentMessage);
                return mValue[mIndex];
            }
        }
        /// <summary>
        /// Parse the integer inside timestamp
        /// </summary>
        /// <param name="value">Integer value</param>
        /// <returns>Integer length in string</returns>
        private int ParseIntWithLength(out int value)
        {
            int index = mIndex;
            value = 0;
            while (index < mValue.Length && char.IsDigit(mValue[index])) index++;
            if (index == mIndex)
                return 0;
            if (int.TryParse(mValue.AsSpan()[mIndex..index], out value))
            {
                int length = index - mIndex;
                mIndex = index;
                return length;
            }
            else
                throw new IllegalContentException($"Integer parsing error, context: {IllegalContentMessage}");
        }
        private void ThrowIfUnreadableNow(string message)
        {
            if (mIndex >= mValue.Length)
                throw new IllegalContentException($"Attempting to consume more characters yet met with the end of string.{Environment.NewLine}{message}");
        }
        /// <summary>
        /// There are three kinds of timestamp in lrc now, we need to distinguish them
        /// </summary>
        /// <returns>The correct representation in <see cref="TimeSpan"/></returns>
        private TimeSpan ParseTimestamp()
        {
            if (ParseIntWithLength(out int minutes) == 0)
                throw new IllegalContentException($"Minutes in timestamp parsing error, {IllegalContentMessage}");

            if (NowCharChecked != ':')
                throw new IllegalContentException($"Expected ':', {IllegalContentMessage}");
            mIndex++;

            if (ParseIntWithLength(out int seconds) == 0)
                throw new IllegalContentException($"Seconds in timestamp parsing error, {IllegalContentMessage}");
            var time = new TimeSpan(0, minutes, seconds);

            if (NowCharChecked == ':' || NowCharChecked == ',' || NowCharChecked == '.')
            {
                mIndex++;
                ThrowIfUnreadableNow(IllegalContentMessage);
                int length;
                if ((length = ParseIntWithLength(out int miliseconds)) == 0)
                    throw new IllegalContentException($"Miliseconds in timestamp parsing error, {IllegalContentMessage}");
                //Need to multiply 10
                if (length == 2)
                    time += new TimeSpan(0, 0, 0, 0, miliseconds * 10);
                else
                    time += new TimeSpan(0, 0, 0, 0, miliseconds);
                return time;
            }
            else
                return time;              
        }
        private string ParseContentBehind()
        {
            int index = mValue.IndexOf("\r\n", mIndex);
            string ret = null;
            if (index != -1)
            {
                ret = mValue[mIndex..index];
                mIndex = index + 2;
                return ret;
            }
            index = mValue.IndexOf("\r", mIndex);
            if (index != -1)
            {
                ret = mValue[mIndex..index];
                mIndex = index + 1;
                return ret;
            }
            index = mValue.IndexOf("\n", mIndex);
            if (index != -1)
            {
                ret = mValue[mIndex..index];
                mIndex = index + 1;
                return ret;
            }
            ret = mValue[mIndex..];
            mIndex = mValue.Length;
            return ret;
        }
        private Action<string, string> GetPropertySetter(out string tagName)
        {
            int splitIndex = mValue.IndexOf(':', mIndex);
            if (splitIndex == -1)
                throw new IllegalContentException($"Unxpected end, {IllegalContentMessage}");
            tagName = mValue[mIndex..splitIndex];
            mIndex = splitIndex + 1;
            foreach (var pair in mPropertyInput)
                if (pair.Key == tagName)
                    return (_, value) => pair.Value(value);

            return (key, value) => mContent.Context[key] = value;
        }
        private void ParseContent()
        {
            while(mIndex < mValue.Length)
            {
                while (mIndex < mValue.Length && char.IsWhiteSpace(mValue[mIndex])) mIndex++;
                if (NowCharChecked == '[')
                {
                    mIndex++;
                    if (char.IsDigit(NowCharChecked))
                    {
                        var time = ParseTimestamp();
                        if (NowCharChecked != ']')
                            throw new IllegalContentException($"Expected ']', {IllegalContentMessage}");
                        else
                            mIndex++;
                        //mutiple timestamp in one line like this: [xx:xx.xxx][yy.yy.yyy] ohhhhh
                        var timestamps = new List<TimeSpan>() { time };
                        while (NowCharChecked == '[')
                        {
                            mIndex++;
                            var newTime = ParseTimestamp();
                            timestamps.Add(newTime);
                            if (NowCharChecked != ']')
                                throw new IllegalContentException($"Expected ']', {IllegalContentMessage}");
                            else
                                mIndex++;
                        }
                        //parse the content
                        string content = ParseContentBehind();
                        mLines.AddRange(timestamps.Select(x => (x, content)));
                    }
                    else
                    {
                        //some other tags.
                        ThrowIfUnreadableNow(IllegalContentMessage);
                        var setter = GetPropertySetter(out string tag);
                        int endIndex = mValue.IndexOf(']', mIndex);
                        if (endIndex == -1)
                            throw new IllegalContentException($"Expected ']', {IllegalContentMessage}");
                        string content = mValue[mIndex..endIndex];
                        setter(tag, content);
                        mIndex = endIndex + 1;
                    }
                }
                else
                    throw new IllegalContentException($"Expected '[', {IllegalContentMessage}");
            }  
        }
        
        public LyricsContent Parse()
        {
            ParseContent();
            var sorted = mLines.OrderBy(x => x.Time).ToArray();
            mContent.Lines = new LyricsLine[sorted.Length];
            for (int i = 0; i < sorted.Length - 1; ++i)
            {
                mContent.Lines[i] = new LyricsLine
                {
                    Content = sorted[i].Content,
                    StartTime = sorted[i].Time,
                    StopTime = sorted[i + 1].Time
                };
            }
            int lastIndex = sorted.Length - 1;
            mContent.Lines[lastIndex] = new LyricsLine
            {
                Content = sorted[lastIndex].Content,
                StartTime = sorted[lastIndex].Time,
                StopTime = sorted[lastIndex].Time + new TimeSpan(0, 0, 2)
            };
            return mContent;
        }
    }
}
