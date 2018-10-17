using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Structurizr.InfrastructureAsCode.Azure.InfrastructureRendering
{
    public static class Powershell
    {
        public static string Run(string file, params KeyValuePair<string, string>[] parameters)
        {
            string script;

            using (var fileStream = typeof(Powershell).Assembly.GetManifestResourceStream(file))
            using (var reader = new StreamReader(fileStream))
            {
                script = reader.ReadToEnd();
            }

            foreach (var parameter in parameters)
            {
                script = script.Replace(parameter.Key, parameter.Value);
            }

            var startInfo = new ProcessStartInfo("powershell", script)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false
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