namespace ExtjsDefinitionGenerator.Helpers
{
    using Configuration;
    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

    public static class Loader
    {
        public static List<Component> Load(string path)
        {
            var files = FindJsonFiles(path);
            var jsonFiles = LoadJsonFiles(files);
            return jsonFiles;
        }

        private static string[] FindJsonFiles(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            var dirs = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);

            return dirs;
        }

        private static List<Component> LoadJsonFiles(IEnumerable<string> files)
        {
            List<Component> jsonFiles = new List<Component>();

            JsonSerializerSettings jss = new JsonSerializerSettings();

            jss.MissingMemberHandling = MissingMemberHandling.Ignore;

            jss.Error = new EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>(delegate (object o, Newtonsoft.Json.Serialization.ErrorEventArgs eea)
            {
                Debug.WriteLine(eea.ErrorContext);
            });

            files.ToList().ForEach(file =>
            {
                string json = System.IO.File.ReadAllText(file, Encoding.GetEncoding(GeneretorConfiguration.Settings.Charset));

                Component convertedFile = (Component)JsonConvert.DeserializeObject<Component>(json, jss);

                if (convertedFile != null)
                    jsonFiles.Add(convertedFile);
            });

            return jsonFiles;
        }
    }
}
