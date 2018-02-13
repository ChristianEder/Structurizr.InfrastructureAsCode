using System;

namespace infrastructurizr.Dependencies
{
    public class ZioTAccelerator : PowershellModuleDependency
    {
        public override int Priority => 4;
        protected override string ModuleName => "zuehlke.iot.powershell";

        protected override void TryToInstall()
        {
            string message;
            Console.Write(" ...trying to install ...");
            Run("git.exe", "clone https://bitbucket.zuehlke.com/scm/ziop/zuehlke.iot.powershell.git -q", out message, "C:\\Program Files\\WindowsPowerShell\\Modules");
        }
    }
}