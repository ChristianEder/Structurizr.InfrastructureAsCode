using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using infrastructurizr.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace infrastructurizr.Util
{
    public static class Powershell
    {
        public static JObject RunScriptOf(Command command, params KeyValuePair<string, string>[] parameters)
        {
            return Run(command.GetType().FullName + ".ps1", parameters);
        }

        public static JObject Run(string file, params KeyValuePair<string, string>[] parameters)
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
                RedirectStandardOutput = true
            };
            var p = Process.Start(startInfo);
            p.WaitForExit();
            var e = p.StandardOutput.ReadToEnd();

            if (p.ExitCode != 0)
            {
                throw new Exception(e);
            }

            return JObject.Parse(e.Trim());
        }
    }
}
