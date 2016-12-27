﻿using System;
using Cassette.Utilities;

namespace Cassette
{
    /// <summary>
    /// Prepends the virtual directory to the beginning of application relative URL paths.
    /// </summary>
    public class VirtualDirectoryPrepender : IUrlModifier
    {
        readonly string virtualDirectory;

        public VirtualDirectoryPrepender(string virtualDirectory)
        {
            if (string.IsNullOrEmpty(virtualDirectory) ||
                !virtualDirectory.StartsWithCharacter('/'))
            {
                throw new ArgumentException("Virtual directory must start with a forward slash.");
            }

            if (virtualDirectory.EndsWith("/"))
            {
                this.virtualDirectory = virtualDirectory;
            }
            else
            {
                this.virtualDirectory = virtualDirectory + "/";
            }
        }

        /// <summary>
        /// Prepends the virtual directory to the beginning of the application relative URL path.
        /// </summary>
        public string Modify(string url)
        {
            return virtualDirectory + url.TrimStart('/');
        }
    }
}