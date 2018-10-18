using System;
using System.Diagnostics;

namespace infrastructurizr.Util
{
    public static class Execute
    {
        public static string Command(string command, string arguments)
        {
            var startInfo = new ProcessStartInfo(command, arguments)
            {
                RedirectStandardOutput = true
            };
            var p = Process.Start(startInfo);
            p.WaitForExit();
            var e = p.StandardOutput.ReadToEnd();

            if (p.ExitCode != 0)
            {
                throw new Exception(e);
            }

            return e.Trim();
        }
    }
}