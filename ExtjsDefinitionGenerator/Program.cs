namespace ExtjsDefinitionGenerator
{
    using Arguments;
    using Helpers;
    using Models;
    using Processors;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    class Program
    {
        static void Main(string[] args)
        {
            var currentLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            try
            {
                CommandLineParser.ReadArgs(args);

                if (string.IsNullOrEmpty(Args.Current.Path))
                    Args.Current.Path = currentLocation + "\\input";

                if (string.IsNullOrEmpty(Args.Current.OutPath))
                    Args.Current.OutPath = currentLocation + "\\output";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CommandLineParser.ShowHelp();
            }

            var path = Args.Current.Path;
            var output = Args.Current.OutPath;
            var tool = currentLocation + "\\Compiler\\jsduck.exe";

            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);

            var command = string.Format("--verbose --output \"{0}\" \"{1}\" --ignore-global --export=full --encoding=iso-8859-1 ", path, path);

            CommandExecutor.ExecuteCommand(tool, command);

            //jsduck --verbose --output ext-5.0.0-json/ ext-5.0.0/src --ignore-global --export=full --encoding=iso-8859-1

            List<Component> jsonFiles = Loader.Load(path);

            var orderedFiles = jsonFiles.OrderBy(a => a.Name).ToList();

            LibraryProcessor.Process(orderedFiles);

            Console.WriteLine("Geração concluída, precione qualquer tecla para continuar.");
            Console.ReadLine();
        }
    }
}
