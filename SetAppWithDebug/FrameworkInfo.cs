namespace SetAppWithDebug
{
    public class FrameworkInfo : VersionedName
    {
        public override string ToString()
        {
            return $"{Name}{Version}";
        }
    }
}
