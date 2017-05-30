namespace ExtjsDefinitionGenerator.Processors
{
    using Configuration;
    using Helpers;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PropertyProcessor
    {
        public DefinitionWriter definitionWriter;

        public PropertyProcessor(DefinitionWriter definitionWriter)
        {
            this.definitionWriter = definitionWriter;
        }

        public List<string> WriteProperties(Component jsonFile, bool isInterface, bool useExport, bool isDeclare)
        {
            List<string> processedConfigNames = new List<string>();

            var configs = jsonFile.Members.Where(m => m.Tagname == "cfg");
            var properties = jsonFile.Members.Where(m => m.Tagname == "property");

            string tab = isDeclare ? "\t" : "\t\t";

            string optionalFlag = isInterface ? "?" : "";
            string exportString = useExport ? "export var " : "";

            if (!useExport && !isInterface && jsonFile.Singleton)
                exportString = "static ";

            foreach (var classConfig in configs)
            {
                if (classConfig.Owner == jsonFile.Name && classConfig.Name != "")
                {
                    if (GeneretorConfiguration.Settings.IncludePrivate == classConfig.Private || GeneretorConfiguration.Settings.IncludePrivate)
                    {
                        if (classConfig.Static && !isInterface)
                            exportString = "static ";

                        if (!SpecialCases.shouldRemoveProperty(jsonFile.Name, classConfig.Name))
                        {
                            string thisType = classConfig.Type;
                            string overrideType = SpecialCases.getPropertyTypeOverride(jsonFile.Name, classConfig.Name);

                            if (!string.IsNullOrEmpty(overrideType))
                                thisType = overrideType;

                            // Property type conversions
                            if (thisType.Contains("/") || thisType.Contains("|"))
                            {
                                string[] types = thisType.Split(new char[] { '/', '|' }, StringSplitOptions.RemoveEmptyEntries);

                                for (int i = 0; i < types.Count(); i++)
                                    types[i] = TypeManager.NormalizeType(TypeManager.ConvertToInterface(types[i]));

                                thisType = string.Join(" | ", types);

                                if (types.Contains("any") || types.Contains("any[]"))
                                    thisType = "any";
                            }
                            else
                                thisType = TypeManager.NormalizeType(TypeManager.ConvertToInterface(thisType));

                            definitionWriter.WriteToDefinition("{0}/** [Config Option] ({1}) {2} */", tab, classConfig.Type, definitionWriter.FormatCommentText(classConfig.ShortDoc));
                            definitionWriter.WriteToDefinition("{0}{1}{2}{3}: {4}", tab, exportString, classConfig.Name.Replace("-", ""), optionalFlag, thisType);
                        }

                        processedConfigNames.Add(classConfig.Name);
                    }
                }
            }

            foreach (var classProperty in properties)
            {
                if ((!isInterface && jsonFile.Singleton) || (classProperty.Owner == jsonFile.Name && classProperty.Name != ""))
                {
                    if (GeneretorConfiguration.Settings.IncludePrivate == classProperty.Private || GeneretorConfiguration.Settings.IncludePrivate)
                    {
                        if (classProperty.Static && !isInterface)
                            exportString = "static ";

                        if (!processedConfigNames.Contains(classProperty.Name) && !SpecialCases.shouldRemoveProperty(jsonFile.Name, classProperty.Name))
                        {
                            string thisType = classProperty.Type;
                            string overrideType = SpecialCases.getPropertyTypeOverride(jsonFile.Name, classProperty.Name);

                            if (!string.IsNullOrEmpty(overrideType))
                            {
                                thisType = overrideType;
                            }

                            // Property type conversions
                            if (thisType.Contains("/") || thisType.Contains("|"))
                            {
                                string[] types = thisType.Split(new char[] { '/', '|' }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0; i < types.Count(); i++)
                                {
                                    types[i] = TypeManager.NormalizeType(TypeManager.ConvertToInterface(types[i]));
                                }

                                if (!types.Contains("any"))
                                    thisType = string.Join(" | ", types);
                                else
                                    thisType = "any";
                            }
                            else
                                thisType = TypeManager.NormalizeType(TypeManager.ConvertToInterface(thisType));

                            definitionWriter.WriteToDefinition("{0}/** [Property] ({1}) {2} */", tab, classProperty.Type, definitionWriter.FormatCommentText(classProperty.ShortDoc));
                            definitionWriter.WriteToDefinition("{0}{1}{2}{3}: {4}", tab, exportString, classProperty.Name.Replace('-', ' '), optionalFlag, thisType);
                        }

                        processedConfigNames.Add(classProperty.Name);
                    }
                }

            }

            return processedConfigNames;
        }
    }
}
