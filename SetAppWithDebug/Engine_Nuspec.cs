//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Xml;
//using System.Xml.Linq;
//using System.Xml.XPath;

//namespace SetAppWithDebug
//{
//    public partial class Engine
//    {
//        private void _processNuspec(Context context, VersionedName current)
//        {
//            var nuspecPath = Path.Combine(context.NugetFolderRoot
//                    , "packages"
//                    , current.Name
//                    , current.Version.ToString()
//                    , $"{current.Name}.nuspec");

//            if (!File.Exists(nuspecPath))
//            {
//                context.Errors.Add($"Unable to find nuspec file at '{nuspecPath}.'  Dependencies cannot be processed for this nuget.");
//                return;
//            }

//            var xDependencyList = _getDependencyElements(nuspecPath);

//            if (!xDependencyList.Any()) return;

//            var applicableDependencies = _getApplicableDependencyElements(xDependencyList, context);
//            _addDependenciesToContext(applicableDependencies, context);
//        }

//        private XElement[] _getDependencyElements(string nuspecPath)
//        {
//            var nuspecContent = File.ReadAllText(nuspecPath);
//            var xNuspec = XDocument.Parse(nuspecContent);
//            XNamespace xNamespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd";

//            var nsManager = new XmlNamespaceManager(new NameTable());
//            nsManager.AddNamespace("default", "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd");

//            return xNuspec.XPathSelectElements("//default:dependency", nsManager).ToArray();
//        }

//        private XElement[] _getApplicableDependencyElements(XElement[] dependencyElements, Context context)
//        {
//            var grouped = dependencyElements.First().Parent.Name.LocalName
//                .Equals("group", StringComparison.CurrentCultureIgnoreCase);
//            List<XElement> applicableDependencies;

//            if (!grouped)
//            {
//                applicableDependencies = new List<XElement>();
//                applicableDependencies.AddRange(dependencyElements);
//            }
//            else
//            {
//                applicableDependencies = _getDependenciesFromGroups(dependencyElements, context);
//            }

//            return applicableDependencies.ToArray();
//        }

//        private List<XElement> _getDependenciesFromGroups(XElement[] dependencyElements, Context context)
//        {
//            var groups = new Dictionary<string, XElement>();
//            var parents = dependencyElements.Select(x => x.Parent);
//            var applicableDependencies = new List<XElement>();

//            foreach (var p in parents)
//            {
//                var framework = p.Attribute("targetFramework")?.Value ?? string.Empty;
//                if (!groups.ContainsKey(framework))
//                    groups.Add(framework, p);
//            }

//            var matchingGroup = groups.FirstOrDefault(x => x.Key.Equals($".{context.ProjectFramework}",
//                StringComparison.InvariantCultureIgnoreCase)).Value;

//            if (matchingGroup == null && groups.ContainsKey(string.Empty))
//                matchingGroup = groups[string.Empty];

//            if (matchingGroup != null)
//                applicableDependencies.AddRange(matchingGroup.Descendants());

//            return applicableDependencies;
//        }

//        private void _addDependenciesToContext(XElement[] dependencies, Context context)
//        {
//            foreach (var dependency in dependencies)
//            {
//                var name = dependency.Attribute("id").Value;
//                var version = Version.Parse(dependency.Attribute("version").Value);

//                if (!context.SearchRegex.IsMatch(name) || context.TargetNugets.Any(x => x.Name.Equals(name)))
//                    continue;

//                context.TargetNugets.Add(new VersionedName
//                {
//                    Name = name,
//                    Version = version
//                });
//            }
//        }
//    }
//}
