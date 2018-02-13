namespace infrastructurizr.Dependencies
{
    public class Git : Dependency
    {
        public override int Priority => 0;
        public override string Name => "Git for Windows";

        protected override bool IsInstalled(out string version)
        {
            return Run("git.exe", "--version", out version);
        }
    }
}