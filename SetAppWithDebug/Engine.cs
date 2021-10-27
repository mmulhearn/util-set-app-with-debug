using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using NuGet;

namespace SetAppWithDebug
{
    public partial class Engine
    {
        public void SwapDebugDlls(Context context)
        {
            context.TargetNugets = _getProjectNugets(context).ToList();
            _processDependencies(context);
            _processNugets(context);
        }

        private IEnumerable<VersionedName> _getProjectNugets(Context context)
        {
            var csProjContent = File.ReadAllText(context.ProjectPath);
            var XDoc = XDocument.Parse(csProjContent);
            var list = new List<VersionedName>();

            context.ProjectFramework = XDoc.XPathSelectElement("Project/PropertyGroup/TargetFramework").Value;

            var assemblyNameElement = XDoc.XPathSelectElement("Project/PropertyGroup/AssemblyName");
            if (assemblyNameElement == null)
                context.ProjectAssemblyName = Path.GetFileNameWithoutExtension(context.ProjectPath);
            else
                context.ProjectAssemblyName = assemblyNameElement.Value;

            var packageRefs = XDoc.XPathSelectElements("Project/ItemGroup/PackageReference");
            foreach (var @ref in packageRefs)
            {
                var package = @ref.Attribute("Include").Value;
                if (context.SearchRegex != null && !context.SearchRegex.IsMatch(package)) continue;
                var version = @ref.Attribute("Version").Value;
                list.Add(new VersionedName
                {
                    Name = package,
                    Version = SemanticVersion.Parse(version)
                });
            }

            return list;
        }

        private void _processNugets(Context context)
        {
            for (var i = 0; i < context.TargetNugets.Count; i++)
            {
                var current = context.TargetNugets[i];

                if (!_processNuget(current, context))
                {
                    continue;
                }

                context.AlteredNugets.Add(current);
            }
        }

        private bool _processNuget(VersionedName nuget, Context context)
        {
            var libPath = $@"{context.NugetFolderRoot}\packages\{nuget.Name}\{nuget.Version}\lib";
            var nugetFramework = _getTargetNugetFramework(libPath, context);

            if (nugetFramework == null)
            {
                context.Errors.Add($"Unable to find appropriate package for '{nuget.Name}, {nuget.Version}.'");
                return false;
            }

            var libFrameworkPath = Path.Combine(libPath, nugetFramework.ToString());

            if (!Directory.Exists(libFrameworkPath))
            {
                context.Errors.Add($"Path '{libFrameworkPath}' does not exist.");
                return false;
            }

            return _swapDebugDlls(libFrameworkPath, nugetFramework, context);
        }

        private bool _swapDebugDlls(string nugetFrameworkDirectory, FrameworkInfo nugetFramework, Context context)
        {
            var dlls = Directory.GetFiles(nugetFrameworkDirectory, "*.dll");
            foreach (var dll in dlls)
            {
                var fileInfo = new FileInfo(dll);

                var root = nugetFramework.Name.Contains("core")
                    ? context.SharedCoreLibraryRoot
                    : context.SharedStandardLibraryRoot;

                var sourcePath = $@"{root}\{fileInfo.Name.Replace(".dll", string.Empty)}\bin\Debug\{nugetFramework}\{fileInfo.Name}";
                if (!File.Exists(sourcePath))
                {
                    context.Errors.Add($"Path '{sourcePath}' does not exist.");
                    return false;
                }

                var backup = $"{dll}.orig";
                if (!File.Exists(backup))
                {
                    File.Move(dll, backup);
                }

                File.Copy(sourcePath, dll, true);
            }

            return true;
        }
    }
}
