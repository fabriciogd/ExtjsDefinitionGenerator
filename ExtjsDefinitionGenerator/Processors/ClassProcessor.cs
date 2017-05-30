namespace ExtjsDefinitionGenerator.Processors
{
    using Configuration;
    using Helpers;
    using Models;
    using System.Collections.Generic;

    public class ClassProcessor
    {
        public DefinitionWriter definitionWriter;
        public PropertyProcessor propertyProcessor;
        public MethodProcessor methodProcessor;

        public ClassProcessor(DefinitionWriter definitionWriter)
        {
            this.definitionWriter = definitionWriter;
            this.propertyProcessor = new PropertyProcessor(definitionWriter);
            this.methodProcessor = new MethodProcessor(definitionWriter);
        }

        public void Process(string name, Component jsonFile)
        {
            bool hasStaticMethods = false;
            List<string> processedNames;

            definitionWriter.WriteToDefinition("\texport interface I{0} {1} {{", TypeManager.GetClassName(name), TypeManager.GetExtends(jsonFile, true));

            if (!jsonFile.Singleton)
            {
                processedNames = WriteProperties(jsonFile, true, false, false);
                hasStaticMethods = WriteMethods(jsonFile, processedNames, true, false, false, false);
            }

            definitionWriter.WriteToDefinition("\t}");

            if (!GeneretorConfiguration.Settings.InterfaceOnly || jsonFile.Singleton || hasStaticMethods)
            {
                if (!GeneretorConfiguration.Settings.InterfaceOnly)
                {
                    definitionWriter.WriteToDefinition("\texport class {0} {1} implements {2} {{", TypeManager.GetClassName(name), TypeManager.GetExtends(jsonFile, false), TypeManager.GetImplementedInterfaces(jsonFile, false));
                    processedNames = WriteProperties(jsonFile, false, false, false);
                }
                else
                {
                    definitionWriter.WriteToDefinition("\texport class {0} {{", TypeManager.GetClassName(name));
                    processedNames = new List<string>();

                    if (jsonFile.Singleton)
                    {
                        processedNames = WriteProperties(jsonFile, false, false, false);
                    }

                }

                WriteMethods(jsonFile, processedNames, false, false, hasStaticMethods, false);

                definitionWriter.WriteToDefinition("\t}");
            }
        }

        public List<string> WriteProperties(Component jsonFile, bool isInterface, bool useExport, bool isDeclare)
        {
            return propertyProcessor.WriteProperties(jsonFile, isInterface, useExport, isDeclare);
        }

        public bool WriteMethods(Component jsonFile, List<string> processedNames, bool isInterface, bool useExport, bool staticOnly, bool isDeclare)
        {
            return methodProcessor.WriteMethods(jsonFile, processedNames, isInterface, useExport, staticOnly, isDeclare);
        }
    }
}
