using System;
using System.IO;

namespace Server.Services.LLM.Core
{
    /// <summary>
    /// <summary>
    /// Provides base directory paths for LLM services
    /// </summary>
    public static class BaseDirectory
    {
        /// <summary>
        /// Gets the base directory for the ServUO installation
        /// </summary>
        public static string Directory
        {
            get
            {
                // Use the current working directory (usually the ServUO root)
                return Environment.CurrentDirectory;
            }
        }

        /// <summary>
        /// Gets the Data directory path
        /// </summary>
        public static string DataDirectory
        {
            get
            {
                return Path.Combine(Directory, "Data");
            }
        }

        /// <summary>
        /// Gets the Config directory path
        /// </summary>
        public static string ConfigDirectory
        {
            get
            {
                return Path.Combine(Directory, "Config");
            }
        }

        /// <summary>
        /// Gets the Scripts directory path
        /// </summary>
        public static string ScriptsDirectory
        {
            get
            {
                return Path.Combine(Directory, "Scripts");
            }
        }
    }
}
