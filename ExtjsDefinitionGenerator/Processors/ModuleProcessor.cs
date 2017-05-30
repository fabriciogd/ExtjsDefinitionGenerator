namespace ExtjsDefinitionGenerator.Processors
{
    using Helpers;
    using Models;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class ModuleProcessor
    {
        public DefinitionWriter definitionWriter;
        public ClassProcessor classProcessor;

        public ModuleProcessor()
        {
            definitionWriter = new DefinitionWriter();
            classProcessor = new ClassProcessor(definitionWriter);
        }

        public void Process(string name, Component jsonFile, bool isAlternate = false)
        {
            var parts = name.Split('.').Length;

            string result = Regex.Replace(jsonFile.Doc, "<(.|\n)*?>", "").Replace("\n", "\n * ");

            result = Regex.Replace(result, @"\s?\*?\*(?=[^*]*$)", "");

            definitionWriter.WriteToDefinition("/**\n * {0}*/", result);

            if ((!isAlternate && name != "Ext") || (isAlternate && parts > 1))
            {
                definitionWriter.WriteToDefinition("declare module {0} {{", TypeManager.GetModule(name));

                classProcessor.Process(name, jsonFile);
            }
            else
            {
                definitionWriter.WriteToDefinition("declare class {0} {{", name);

                List<string> processedNames = classProcessor.WriteProperties(jsonFile, false, false, true);

                classProcessor.WriteMethods(jsonFile, processedNames, false, false, false, true);
            }

            definitionWriter.WriteToDefinition("}");
        }
    }
}
