using System.Collections.Generic;
using System.IO;
using infrastructurizr.Commands;
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

            return JObject.Parse(Execute.Command("powershell", script));
        }
    }
}
