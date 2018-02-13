namespace infrastructurizr.Dependencies
{
    public class Powershell : Dependency
    {
        public override int Priority => 1;
        public override string Name => "Powershell";

        protected override bool IsInstalled(out string version)
        {
            return Run("powershell.exe", "\"$PSVersionTable.PSVersion.Major.ToString() + '.' + $PSVersionTable.PSVersion.Minor.ToString()\"", out version);
        }
    }
}