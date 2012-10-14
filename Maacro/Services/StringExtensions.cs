using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maacro.Services
{
    internal static class StringExtensions
    {
        public static void Add(this IList<string> source, string format, params object[] args)
        {
            source.Add(string.Format(format, args));
        }
    }
}
