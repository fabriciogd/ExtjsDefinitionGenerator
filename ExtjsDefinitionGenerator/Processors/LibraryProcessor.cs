namespace ExtjsDefinitionGenerator.Processors
{
    using Helpers;
    using Models;
    using System;
    using System.Collections.Generic;

    public static class LibraryProcessor
    {
        public static void Process(List<Component> jsonFiles)
        {
            ModuleProcessor moduleProcessor = new ModuleProcessor();

            foreach (var jsonFile in jsonFiles)
            {
                if (jsonFile.Name.Contains("Ext") || TypeManager.IsCustomNamespace(jsonFile.Name))
                {
                    Console.WriteLine("Processando {0}", jsonFile.Name);

                    moduleProcessor.Process(jsonFile.Name, jsonFile);

                    if (jsonFile.AlternateClassNames.Count > 0)
                    {
                        foreach (var alternateClassName in jsonFile.AlternateClassNames)
                        {
                            if (alternateClassName.ToLower() != jsonFile.Name.ToLower())
                                moduleProcessor.Process(alternateClassName, jsonFile, true);
                        }
                    }
                }
            }
        }
    }
}
