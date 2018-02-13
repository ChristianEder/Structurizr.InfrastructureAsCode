using System;

namespace infrastructurizr.Dependencies
{
    public abstract class PowershellModuleDependency : Dependency
    {
        protected abstract string ModuleName { get; }

        public sealed override string Name => "Powershell Module: " + ModuleName;

        protected sealed override bool IsInstalled(out string version)
        {
            var isInstalled = CheckInstallation(out version);
            if (!isInstalled)
            {
                TryToInstall();
                isInstalled = CheckInstallation(out version);
            }
            return isInstalled;
        }

        private bool CheckInstallation(out string version)
        {
            return Run("powershell.exe",
                $"\"(Get-Module -ListAvailable -Name {ModuleName})[0].Version.Major.ToString() + '.' + (Get-Module -ListAvailable -Name {ModuleName})[0].Version.Minor.ToString()\"",
                out version);
        }

        protected virtual void TryToInstall()
        {
            string message;
            Console.Write(" ...trying to install ...");
            Run("powershell.exe", "Install-Module " + ModuleName, out message);
        }
    }
}