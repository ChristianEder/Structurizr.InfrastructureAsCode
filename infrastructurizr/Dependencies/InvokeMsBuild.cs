namespace infrastructurizr.Dependencies
{
    public class InvokeMsBuild : PowershellModuleDependency
    {
        public override int Priority => 3;

        protected override string ModuleName => "Invoke-MsBuild";
    }
}