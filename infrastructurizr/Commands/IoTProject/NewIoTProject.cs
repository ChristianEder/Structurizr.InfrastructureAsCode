using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using infrastructurizr.Util;

namespace infrastructurizr.Commands.IoTProject
{
    public class NewIoTProject : Command
    {
        public override string Name => "new iotproject";
        public override string Description => "Creates a new iot architecture project from one of the templates provided in https://github.com/ChristianEder/azure-iot-reference-architectures.";

        public StringParameter TargetFolder { get; }
        public StringParameter ProjectName { get; }
        public StringParameter TmpFolder { get; }

        public NewIoTProject()
        {
            TargetFolder = new StringParameter("targetFolder", "Defines the folder in which the project will be created. In that directory, a subfolder will be created for the project. The default value is the current working directory.");
            ProjectName = new StringParameter("name", "The name of the project to be created. This will be used as a folder name for the generated project and also replace the namespaces in the project template. As a default, the name of the selected template will be used.");
            TmpFolder = new StringParameter("tmp", $"The tmp folder will be used to clone the available templates to. The default value is {Path.GetTempPath()}");
        }


        public override void Execute()
        {
            var cwd = Directory.GetCurrentDirectory();

            try
            {
                var target = TargetFolder.HasValue ? TargetFolder.Value : cwd;

                if (!Directory.Exists(target))
                {
                    using (new TemporaryConsoleColor(ConsoleColor.Red))
                    {
                        Console.WriteLine($"Target folder does not exist: {target}");
                    }
                    return;
                }

                var tmp = TmpFolder.HasValue ? TmpFolder.Value : Path.GetTempPath();
                if (!Directory.Exists(tmp))
                {
                    using (new TemporaryConsoleColor(ConsoleColor.Red))
                    {
                        Console.WriteLine($"Tmp folder does not exist: {target}");
                    }
                    return;
                }
                var referenceArchitecturesFolder = Path.Combine(tmp, "azure-iot-reference-architectures");

                try
                {
                    Directory.SetCurrentDirectory(tmp);

                    if (Directory.Exists(referenceArchitecturesFolder))
                    {
                        DeleteFolder(referenceArchitecturesFolder);
                    }
                    Util.Execute.Command("git", "clone https://github.com/ChristianEder/azure-iot-reference-architectures.git");

                    Directory.SetCurrentDirectory(referenceArchitecturesFolder);

                    var architectures = Directory.GetDirectories(referenceArchitecturesFolder)
                        .Select(d => new DirectoryInfo(d))
                        .Where(d => d.Name != ".git").ToArray();

                    Console.WriteLine("Please select a template to be used. (Enter to proceed, Esc to cancel)");

                    var selected = MultipleChoice.Ask(null, architectures.Select(a => a.Name).ToArray());

                    if (string.IsNullOrEmpty(selected))
                    {
                        return;
                    }

                    var templateFolder = architectures.Single(a => a.Name == selected);

                    Copy(templateFolder, target);
                }
                finally
                {
                    Directory.SetCurrentDirectory(cwd);

                    DeleteFolder(referenceArchitecturesFolder);
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(cwd);
            }

        }

        private static void DeleteFolder(string folder)
        {
            foreach (string filePath in Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
            }

            Directory.Delete(folder, true);
        }

        private void Copy(DirectoryInfo templateFolder, string target)
        {
            var name = ProjectName.HasValue ? ProjectName.Value : templateFolder.Name;
            string targetFolder = Path.Combine(target, name);

            if (Directory.Exists(targetFolder))
            {
                DeleteFolder(targetFolder);
            }
            Directory.CreateDirectory(targetFolder);

            foreach (string dirPath in Directory.GetDirectories(templateFolder.FullName, "*",
                SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(templateFolder.FullName, targetFolder));
            }

            foreach (string newPath in Directory.GetFiles(templateFolder.FullName, "*.*",
                SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(templateFolder.FullName, targetFolder), true);
            }

            if (name != templateFolder.Name)
            {
                foreach (var file in Directory.GetFiles(targetFolder, "*.*", SearchOption.AllDirectories))
                {
                    var fileInfo = new FileInfo(file);

                    File.WriteAllText(file,  File.ReadAllText(file).Replace(templateFolder.Name, name));

                    if (fileInfo.Name.Contains(templateFolder.Name))
                    {
                        File.Move(file, Path.Combine(fileInfo.DirectoryName, fileInfo.Name.Replace(templateFolder.Name, name)));
                    }
                }
            }

            using (new TemporaryConsoleColor(ConsoleColor.Green))
            {
                Console.WriteLine("#### Successfully created " + name + " in " + targetFolder);
            }

            var winDir = Environment.GetEnvironmentVariable("WINDIR");
            if (!string.IsNullOrEmpty(winDir))
            {
                Console.Write("#### Do you want me to open that folder? [y/N] ");
                if (Console.ReadLine()?.ToLowerInvariant() == "y")
                {
                    Process.Start(winDir + @"\explorer.exe", targetFolder);
                }
            }
        }
    }
}
