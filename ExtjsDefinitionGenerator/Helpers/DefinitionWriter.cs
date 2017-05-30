namespace ExtjsDefinitionGenerator.Helpers
{
    using Arguments;
    using Configuration;
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    public class DefinitionWriter
    {
        StreamWriter stream;

        public DefinitionWriter()
        {
            string outFile = string.Format("{0}-{1}-{2}.d.ts",
                GeneretorConfiguration.Settings.Name,
                GeneretorConfiguration.Settings.Version,
                GeneretorConfiguration.Settings.InterfaceOnly ? "Typed-Interfaces" : "Typed");

            string filePath = string.Format(@"{0}\{1}", Args.Current.OutPath, outFile);

            if (File.Exists(filePath))
            {
                File.WriteAllText(filePath, String.Empty);
                stream = new System.IO.StreamWriter(filePath);
            }
            else
            {
                stream = new StreamWriter(File.Create(filePath));
            }

            stream.WriteLine("// Type definitions for {0} {1}", GeneretorConfiguration.Settings.Name, GeneretorConfiguration.Settings.Version);
            stream.WriteLine("// Project: http://www.sencha.com/products/{0} /", GeneretorConfiguration.Settings.Name.ToLower());
            stream.WriteLine("");


            stream.Flush();
        }

        public void WriteToDefinition(string value)
        {
            value = value.Replace("(  ){1,100}", " ");
            stream.WriteLine(value);
            stream.Flush();
        }

        public void WriteToDefinition(string value, params object[] args)
        {
            value = value.Replace("(  ){1,100}", " ");
            stream.WriteLine(value, args);
            stream.Flush();
        }

        public string FormatCommentText(string comment)
        {
            string result = "";

            if (!string.IsNullOrEmpty(comment))
                result += Regex.Replace(comment, "<(.|\n)*?>", "").Replace("\n", " ").Replace("\t", " ").Replace("\r", " ");

            return result;
        }
    }
}
