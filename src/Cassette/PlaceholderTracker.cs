using System;
using System.Collections.Generic;
using System.Linq;

namespace Cassette
{
    class PlaceholderTracker : IPlaceholderTracker
    {
        readonly Dictionary<Guid, Func<string>> creationFunctions = new Dictionary<Guid, Func<string>>();

        public string InsertPlaceholder(Func<string> futureHtml)
        {
            var id = Guid.NewGuid();
            creationFunctions[id] = futureHtml;
            return Environment.NewLine + id + Environment.NewLine;
        }

        public string ReplacePlaceholders(string html)
        {
            string result = html;
            foreach (var function in creationFunctions)
                result = result.Replace(function.Key.ToString(), function.Value());
            return result;
        }
    }
}