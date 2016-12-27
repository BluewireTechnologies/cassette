using System;
using System.Collections.Generic;
using System.IO;

namespace Cassette.Utilities
{
    public class CaseInsensitivePathEqualityComparer : IEqualityComparer<string>
    {
        private static string NormaliseSeparators(string path)
        {
            return path?.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        public bool Equals(string x, string y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x?.Length != y?.Length) return false;
            return StringComparer.OrdinalIgnoreCase.Equals(
                NormaliseSeparators(x),
                NormaliseSeparators(y));
        }

        public int GetHashCode(string obj)
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(NormaliseSeparators(obj));
        }
    }
}
