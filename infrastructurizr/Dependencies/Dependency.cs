using System;
using System.Diagnostics;

namespace infrastructurizr.Dependencies
{
    public abstract class Dependency
    {
        public abstract int Priority { get; }
        public abstract string Name { get; }

        public bool Assert()
        {
            Console.Write("##### " + Name);
            string version;
            var isInstalled = IsInstalled(out version);
            Console.WriteLine(isInstalled ? (" v (" + version.Trim() + ")") : " x");
            return isInstalled;
        }

        protected abstract bool IsInstalled(out string version);

        protected bool Run(string file, string arguments, out string output, string workingDirectory = null)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo(file, arguments)
                {
                    RedirectStandardOutput = true,
                };

                if (!string.IsNullOrEmpty(workingDirectory))
                {
                    processStartInfo.WorkingDirectory = workingDirectory;
                }

                var process = Process.Start(processStartInfo);
                process.WaitForExit();
                output = process.StandardOutput.ReadToEnd();
                return process.ExitCode == 0;
            }
            catch (Exception)
            {
                output = null;
                return false;
            }
        }
    }
}
