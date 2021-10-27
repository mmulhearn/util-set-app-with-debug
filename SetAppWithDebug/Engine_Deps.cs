using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NuGet;

namespace SetAppWithDebug
{
    public partial class Engine
    {
        private void _processDependencies(Context context)
        {
            var projFile = new FileInfo(context.ProjectPath);

            var depsFilePath = Path.Combine(projFile.DirectoryName
                , @"bin\Debug"
                , context.ProjectFramework
                , $"{context.ProjectAssemblyName}.deps.json");

            if (!File.Exists(depsFilePath))
            {
                context.Errors.Add($"Unable to find nuspec file at '{depsFilePath}.'  Dependencies cannot be processed for this nuget.");
                return;
            }

            var depsContents = File.ReadAllText(depsFilePath);
            var jDeps = JObject.Parse(depsContents);
            var jLibraries = (JObject) jDeps["libraries"];

            foreach (JProperty jLib in jLibraries.Children())
            {
                var splitKey = jLib.Name.Split('/');
                var name = splitKey[0];
                var version = splitKey[1];

                if (!context.SearchRegex.IsMatch(name)) continue;

                var jDetails = (JObject) jLib.Value;

                if (!jDetails["type"].Value<string>().Equals("package")) continue;

                if (!context.TargetNugets.Any(x => x.Name.Equals(name)))
                {
                    context.TargetNugets.Add(new VersionedName
                    {
                        Name = name,
                        Version = SemanticVersion.Parse(version)
                    });
                }
            }
        }
    }
}
