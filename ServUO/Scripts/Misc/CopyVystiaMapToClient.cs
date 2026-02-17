using System;
using System.IO;
using Server.Commands;

namespace Server.Misc
{
    /// <summary>
    /// Command to copy Vystia map files to client directory
    /// </summary>
    public class CopyVystiaMapToClient
    {
        public static void Initialize()
        {
            CommandSystem.Register("CopyVystiaToClient", AccessLevel.Administrator, new CommandEventHandler(CopyVystiaToClient_OnCommand));
        }

        [Usage("CopyVystiaToClient [clientpath]")]
        [Description("Copies Vystia map files (map9.mul, staidx9.mul, statics9.mul) from C:\\Vystia_Map to the client directory. If no path provided, uses the first DataDirectory.")]
        private static void CopyVystiaToClient_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            string sourceDir = @"C:\Vystia_Map";
            string targetDir = null;

            // Get target directory
            if (e.Length > 0)
            {
                targetDir = e.GetString(0);
            }
            else if (Core.DataDirectories.Count > 0)
            {
                targetDir = Core.DataDirectories[0];
            }
            else
            {
                from.SendMessage("Error: No client directory specified and no DataDirectories configured.");
                from.SendMessage("Usage: [CopyVystiaToClient [clientpath]");
                from.SendMessage("Example: [CopyVystiaToClient \"C:\\Program Files (x86)\\Electronic Arts\\Ultima Online Classic\"");
                return;
            }

            // Verify source directory
            if (!Directory.Exists(sourceDir))
            {
                from.SendMessage("Error: Source directory not found: {0}", sourceDir);
                return;
            }

            // Verify target directory
            if (!Directory.Exists(targetDir))
            {
                from.SendMessage("Error: Target directory not found: {0}", targetDir);
                from.SendMessage("Please create the directory or specify a valid path.");
                return;
            }

            string[] filesToCopy = { "map9.mul", "staidx9.mul", "statics9.mul" };
            int copied = 0;
            int skipped = 0;
            int errors = 0;

            from.SendMessage("Copying Vystia map files from {0} to {1}...", sourceDir, targetDir);

            foreach (string fileName in filesToCopy)
            {
                string sourcePath = Path.Combine(sourceDir, fileName);
                string targetPath = Path.Combine(targetDir, fileName);

                try
                {
                    if (!File.Exists(sourcePath))
                    {
                        from.SendMessage("Warning: Source file not found: {0}", fileName);
                        errors++;
                        continue;
                    }

                    if (File.Exists(targetPath))
                    {
                        from.SendMessage("File already exists: {0} (skipping)", fileName);
                        skipped++;
                        continue;
                    }

                    File.Copy(sourcePath, targetPath, false);
                    from.SendMessage("Copied: {0}", fileName);
                    copied++;
                }
                catch (Exception ex)
                {
                    from.SendMessage("Error copying {0}: {1}", fileName, ex.Message);
                    errors++;
                }
            }

            from.SendMessage("");
            from.SendMessage("Copy complete: {0} copied, {1} skipped, {2} errors", copied, skipped, errors);
            
            if (copied > 0)
            {
                from.SendMessage("IMPORTANT: Restart your UO client for the map files to be recognized!");
            }
        }
    }
}

