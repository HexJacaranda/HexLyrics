using System;

namespace HexLyrics.Interfaces
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class LyricsComponentAttribute : Attribute
    {
        public LyricsComponentAttribute(string name) { Name = name; }
        public string Name { get; }
    }
}
