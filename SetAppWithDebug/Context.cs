using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SetAppWithDebug
{
    public class Context
    {
        public string SharedStandardLibraryRoot { get; set; }
        public string NugetFolderRoot { get; set; }
        public string SharedCoreLibraryRoot { get; set; }
        public string ProjectPath { get; set; }
        public string ProjectAssemblyName { get; set; }
        public Regex SearchRegex { get; set; }
        public List<VersionedName> TargetNugets { get; set; }
        public string ProjectFramework { get; set; }
        public List<string> Errors { get; set; }
        public List<VersionedName> AlteredNugets { get; set; }

        public Context()
        {
            Errors = new List<string>();
            AlteredNugets = new List<VersionedName>();
        }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>
        ///   <c>true</c> if the specified error is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid(out string error)
        {
            if (string.IsNullOrWhiteSpace(SharedStandardLibraryRoot))
            {
                error = "Standard folder is empty.";
                return false;
            }

            if (!Directory.Exists(SharedStandardLibraryRoot))
            {
                error = "Standard folder does not exist.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(SharedCoreLibraryRoot))
            {
                error = "Core folder is empty.";
                return false;
            }

            if (!Directory.Exists(SharedCoreLibraryRoot))
            {
                error = "Core folder does not exist.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(NugetFolderRoot))
            {
                error = "Nuget folder is empty.";
                return false;
            }

            if (!Directory.Exists(NugetFolderRoot))
            {
                error = "Nuget folder does not exist.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(ProjectPath))
            {
                error = "Project file is empty.";
                return false;
            }

            if (!File.Exists(ProjectPath))
            {
                error = "Project file does not exist.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
