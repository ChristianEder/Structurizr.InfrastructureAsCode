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
            var isInstalled = IsInstalled(out var version);
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = isInstalled ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write("##### " + Name);
            Console.WriteLine(isInstalled ? (" v (" + version.Trim() + ")") : " x");
            Console.ForegroundColor = currentColor;
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
