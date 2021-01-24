using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HexLyrics.Interfaces
{
    public interface ILyricsComponentFactory
    {
        ILyricsParser GenerateParser(string content);
        ILyricsFormatter GenerateFormatter(LyricsContent content);
    }

    public static class LyricsFactory
    {
        private readonly static Dictionary<string, ILyricsComponentFactory> mCache =
            new Dictionary<string, ILyricsComponentFactory>();
        static LyricsFactory()
        {
            foreach (var type in Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => typeof(ILyricsComponentFactory).IsAssignableFrom(x) &&
                x != typeof(ILyricsComponentFactory)))
            {
                var mark = type.GetCustomAttribute<LyricsComponentAttribute>();
                if (mark == null) continue;

                try
                {
                    var instance = Activator.CreateInstance(type) as ILyricsComponentFactory;
                    mCache.Add(mark.Name, instance);
                }
                catch (Exception) { }
            }
        }
        /// <summary>
        /// Construct corresponding factory according to name.
        /// </summary>
        /// <param name="name">Factory name</param>
        /// <returns>factory</returns>
        public static ILyricsComponentFactory From(string name)
            => mCache[name];
    }
}
