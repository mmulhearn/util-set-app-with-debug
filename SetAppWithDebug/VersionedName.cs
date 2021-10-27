using NuGet;

namespace SetAppWithDebug
{
    public class VersionedName
    {
        public string Name { get; set; }

        public SemanticVersion Version { get; set; }

        public override string ToString()
        {
            return $"{Name}.{Version}";
        }
    }
}
