using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NuGet;

namespace SetAppWithDebug
{
    public partial class Engine
    {
        private FrameworkInfo _getTargetNugetFramework(string nugetLibPath, Context context)
        {
            var nugetLibDir = new DirectoryInfo(nugetLibPath);
            var frameworkInfo = _getFrameworkInfo(context.ProjectFramework);

            if (string.IsNullOrWhiteSpace(frameworkInfo?.Name))
                return null;

            var matchingFramework = _getApplicableFrameworkInfo(nugetLibDir, frameworkInfo, frameworkInfo.Name);
            if (matchingFramework != null)
                return matchingFramework;

            // if frameworkInfo is not targeting standard, we should check for them
            if (frameworkInfo.Name.Equals("netstandard")) return null;

            var standardFramework = _getApplicableFrameworkInfo(nugetLibDir, frameworkInfo, "netstandard");
            return standardFramework;
        }

        private FrameworkInfo _getApplicableFrameworkInfo(DirectoryInfo nugetLibraryDirectory, FrameworkInfo targetFrameworkInfo, string frameworkName)
        {
            var matchingDirs = nugetLibraryDirectory.GetDirectories($"{frameworkName}*");
            var dirNames = matchingDirs.Select(x => x.Name);
            var frameworkInfos = _getFrameworkInfo(dirNames);

            foreach (var info in frameworkInfos.OrderByDescending(x => x.Version))
            {
                if (info.Version <= targetFrameworkInfo.Version)
                {
                    return info;
                }
            }

            return null;
        }

        private FrameworkInfo _getFrameworkInfo(string frameworkString)
        {
            var match = Regex.Match(frameworkString, "^[a-z]{1,}");

            if (!match.Success || string.IsNullOrWhiteSpace(match.Value))
                return null;

            var frameworkInfo = new FrameworkInfo
            {
                Name = match.Value
            };

            match = Regex.Match(frameworkString, "([0-9]|\\.){1,}");
            if (match.Success && !string.IsNullOrWhiteSpace(match.Value))
                frameworkInfo.Version = SemanticVersion.Parse(match.Value);

            return frameworkInfo;
        }

        private IEnumerable<FrameworkInfo> _getFrameworkInfo(IEnumerable<string> frameworkStrings)
        {
            foreach (var str in frameworkStrings)
            {
                yield return _getFrameworkInfo(str);
            }
        }
    }
}
