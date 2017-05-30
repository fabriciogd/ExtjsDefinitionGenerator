namespace ExtjsDefinitionGenerator.Arguments
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text.RegularExpressions;

    public class CommandLineParser
    {
        private StringDictionary parameters;

        public StringDictionary Parameters
        {
            get { return parameters; }
        }

        public CommandLineParser(string[] arguments)
        {
            parameters = new StringDictionary();

            Regex spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string parameter = null;
            string[] parts;

            foreach (string argument in arguments)
            {
                parts = spliter.Split(argument, 3);

                switch (parts.Length)
                {
                    case 1:
                        if (parameter != null)
                        {
                            if (!parameters.ContainsKey(parameter))
                            {
                                parts[0] = remover.Replace(parts[0], "$1");
                                parameters.Add(parameter, parts[0]);
                            }
                            parameter = null;
                        }
                        break;
                    case 2:
                        if (parameter != null)
                        {
                            if (!parameters.ContainsKey(parameter)) parameters.Add(parameter, "true");
                        }
                        parameter = parts[1];
                        break;
                    case 3:
                        if (parameter != null)
                            if (!parameters.ContainsKey(parameter)) parameters.Add(parameter, "true");

                        parameter = parts[1];
                        parts[2] = remover.Replace(parts[2], "$1");

                        if (!parameters.ContainsKey(parameter))
                            parameters.Add(parameter, parts[2]);
                        else
                            parameters[parameter] += ";" + parts[2];
                        parameter = null;
                        break;
                }
            }
            if (parameter != null)
            {
                if (!parameters.ContainsKey(parameter))
                    parameters.Add(parameter, "true");
            }
        }

        public string this[string Param]
        {
            get
            {
                return (parameters[Param]);
            }
        }

        public static void ReadArgs(string[] args)
        {
            var parser = new CommandLineParser(args);

            foreach (string key in parser.Parameters.Keys)
            {
                switch (key)
                {
                    case "p":
                    case "path":
                        {
                            var arg = parser[key];
                            if (string.IsNullOrEmpty(arg))
                                throw new ArgumentException("Invalid argument", key);

                            CheckExistingDir(arg);
                            Args.Current.Path = arg;
                            break;
                        }
                    case "op":
                    case "outpath":
                        {
                            var arg = parser[key];
                            if (string.IsNullOrEmpty(arg))
                                throw new ArgumentException("Invalid argument", key);

                            Args.Current.OutPath = arg;
                            break;
                        }
                    case "h":
                    case "help":
                    case "/?":
                        {
                            ShowHelp();
                            Environment.Exit(1);
                            return;
                        }
                    default:
                        {
                            throw new ArgumentException("Invalid argument", key);
                        }
                }
            }
        }

        private static void CheckExistingDir(string arg)
        {
            if (!Directory.Exists(arg))
                throw new DirectoryNotFoundException(arg);
        }

        public static void ShowHelp()
        {
            Console.Out.WriteLine("");
            Console.Out.WriteLine("Uso:");
            Console.Out.WriteLine("\t" + System.AppDomain.CurrentDomain.FriendlyName + " <argumentos>");
            Console.Out.WriteLine(Environment.NewLine + "Arguments:");
            Console.Out.WriteLine("\t--path, -p                                Caminho da pasta dos arquivos para geração");
            Console.Out.WriteLine("\t--outpath, -op                            Caminho da pasta para criação do arquivo gerado");
            Console.Out.WriteLine(Environment.NewLine + "Help:");
            Console.Out.WriteLine("\t--help, -h        Mostrar a ajuda.");
            Console.Out.WriteLine("");
        }
    }
}
