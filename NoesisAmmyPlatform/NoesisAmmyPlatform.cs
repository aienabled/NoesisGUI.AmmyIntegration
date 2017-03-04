namespace NoesisAmmyPlatform
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Ammy;
    using Ammy.Platforms;

    public class NoesisAmmyPlatform : IAmmyPlatform
    {
        private readonly Host host;

        public NoesisAmmyPlatform()
        {
            this.host = new Host(this);
        }

        public string[] DefaultNamespaces => new[] { "Noesis" };

        public string Name => "NoesisGUI";

        public string OutputFileSuffix => "";

        public PlatformTypeNames PlatformTypeNames => NoesisTypeNames.Instance;

        public string[] StaticPropertyImportList { get; }
            = {
                "Noesis.FontStyles",
                "Noesis.FontWeights",
                "Noesis.FontStretches",
                "Noesis.Brushes",
                // not supported in NoesisGUI (yet?)
                //"Noesis.TextDecorations",
                //"Noesis.DashStyles"
            };

        public bool SupportsRuntimeUpdate => false;

        public KeyValuePair<string, string>[] TopNodeAttributes { get; }
            = {
                new KeyValuePair<string, string>("xmlns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"),
                new KeyValuePair<string, string>("xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml")
            };

        public string XPrefix => "x:";

        public Type[] ProvideTypes()
        {
            // In this demo implementation we will simpy return all types
            // from all the assemblies loaded in the current app domain.
            return AppDomain.CurrentDomain
                            .GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .ToArray();
        }

        public void TriggerCompilation(string rootPath, IReadOnlyList<string> sourceFilePaths, string outputDataPath)
        {
            var sources = sourceFilePaths.Select(
                                             path => (Source)new FileSource(path))
                                         .ToArray();

            var compilationRequest = new CompilationRequest(sources);

            var result = this.host.Compile(compilationRequest);

            if (result.IsSuccess)
            {
                this.GenerateFiles(result.Files, rootPath, outputDataPath);
            }

            var messages = result.CompilerMessages;
            if (messages.Any(m => m.Type == CompilerMessageType.Error))
            {
                throw new Exception(
                    "Ammy compilation errors: "
                    + string.Join(Environment.NewLine, messages.Select(FormatMessage)));
            }
        }

        private static string FormatMessage(CompilerMessage m)
        {
            return
                $"[{m.Type}] {m.Message}{Environment.NewLine}    at {m.Location.Filename}:({m.Location.Row},{m.Location.Column})";
        }

        private void GenerateFiles(OutputFile[] files, string rootPath, string outputDataPath)
        {
            if (Directory.Exists(outputDataPath))
            {
                Directory.Delete(outputDataPath, recursive: true);
            }

            Directory.CreateDirectory(outputDataPath);

            foreach (var file in files)
            {
                var path = file.FullPath;
                var localFilePath = path.Substring(rootPath.Length, path.Length - rootPath.Length - ".ammy".Length)
                                    + ".xaml";

                var outputFilePath = Path.Combine(outputDataPath, localFilePath);
                File.WriteAllText(outputFilePath, file.Xaml);
            }
        }
    }
}