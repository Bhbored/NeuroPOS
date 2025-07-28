using FuzzySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.Services
{
    public static class EntityResolver
    {
        public static string? ResolveProduct(string? spokenName, IEnumerable<string> catalog)
        {
            if (string.IsNullOrWhiteSpace(spokenName)) return null;
            var best = Process.ExtractOne(spokenName, catalog);
            return best?.Score >= 80 ? best.Value : null;
        }

        public static string? ResolveContact(string? spokenName, IEnumerable<string> contacts)
        {
            if (string.IsNullOrWhiteSpace(spokenName)) return null;
            var best = Process.ExtractOne(spokenName, contacts);
            return best?.Score >= 80 ? best.Value : null;
        }
    }
}
