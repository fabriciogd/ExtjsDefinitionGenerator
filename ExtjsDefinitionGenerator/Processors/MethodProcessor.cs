namespace ExtjsDefinitionGenerator.Processors
{
    using Configuration;
    using Helpers;
    using Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class MethodProcessor
    {
        public DefinitionWriter definitionWriter;

        public MethodProcessor(DefinitionWriter definitionWriter)
        {
            this.definitionWriter = definitionWriter;
        }

        public bool WriteMethods(Component jsonFile, List<string> processedNames, bool isInterface, bool useExport, bool staticOnly, bool isDeclare)
        {
            List<string> processedMethodNames = new List<string>();

            string optionalFlag = "";

            bool hasStaticMethods = false;
            bool isSingleton = !isInterface && (jsonFile.Singleton || staticOnly);

            var tab = isDeclare ? "\t" : "\t\t";

            var classMethods = jsonFile.Members.Where(m => m.Tagname == "method");

            foreach (var classMethod in classMethods)
            {
                var thisMethod = classMethod;

                if (ShouldIncludeMethod(jsonFile, thisMethod, isSingleton, isInterface, staticOnly, processedNames))
                {
                    if (SpecialCases.shouldRewriteMethod(jsonFile.Name, classMethod.Name))
                        thisMethod = SpecialCases.getRewriteMethod(jsonFile.Name, classMethod.Name, classMethod);

                    processedMethodNames.Add(thisMethod.Name);
                    NormalizeMethodDoc(thisMethod);
                    HandleReturnTypeSpecialCases(thisMethod, jsonFile);

                    optionalFlag = isInterface ? "?" : "";

                    if (SpecialCases.shouldConvertToProperty(jsonFile.Name, thisMethod.Name))
                    {
                        WriteMethodAsProperty(thisMethod, tab, optionalFlag);
                    }
                    else
                    {
                        IterateMethodSignatures(tab, jsonFile, thisMethod, optionalFlag, useExport, isInterface, isSingleton);
                    }
                }

                if (classMethod.Static && !hasStaticMethods && ShouldIncludeMethod(jsonFile, classMethod, isInterface, isSingleton, true, processedNames))
                {
                    hasStaticMethods = true;
                }
            }

            return hasStaticMethods;
        }

        public void IterateMethodSignatures(string tab, Component jsonFile, Members thisMethod, string optionalFlag, bool useExport, bool isInterface, bool isSingleton)
        {
            string[] tokenizedTypes = GeneretorConfiguration.Settings.UseFullTyping ? TypeManager.getTokenizedReturnTypes(thisMethod.Return) : new string[] { "any" };

            foreach (var tokenizedType in tokenizedTypes)
            {
                MethodParameters methodParameters = new MethodParameters(thisMethod);

                bool isStatic = !isInterface && ((isSingleton && (thisMethod.Static || thisMethod.Autodetected.Static))
                                            || thisMethod.Static
                                            || SpecialCases.shouldStatic(jsonFile.Name, thisMethod.Name));

                List<string> usedPermutations = new List<string>();
                bool methodWritten = false;

                if (methodParameters.HasOnlyOneSignature())
                {
                    WriteMethod(tab, thisMethod.ShortDoc, thisMethod.Name, optionalFlag, methodParameters, tokenizedType, thisMethod.Return, useExport, isStatic, isInterface);
                }
                else if (ShouldCreateOverrideMethod(methodParameters.requiresOverrides, tokenizedTypes, tokenizedType))
                {
                    List<string> overrideTypes = methodParameters.paramNames.Select(a => "any").ToList();
                    MethodParameters overriddenMethodParams = methodParameters.CloneWithNewParamTypes(overrideTypes);
                    WriteMethod(tab, thisMethod.ShortDoc, thisMethod.Name, optionalFlag, overriddenMethodParams, "any", thisMethod.Return, useExport, isStatic, isInterface);
                    usedPermutations = overrideTypes;
                    methodWritten = true;
                }

                if (GeneretorConfiguration.Settings.UseFullTyping && usedPermutations.Count > 0)
                {
                    ProcessSignaturePermutations(tab, thisMethod, optionalFlag, tokenizedType, methodParameters, usedPermutations, methodWritten, useExport, isStatic);
                }
            }
        }

        public void ProcessSignaturePermutations(string tab, Members thisMethod, string optionalFlag, string thisType, MethodParameters methodParameters, List<string> usedPermutations, bool methodWritten, bool useExport, bool isStatic)
        {
            List<string> paramPermutations = methodParameters.paramTypes;

            if (!methodParameters.requiresOverrides || (methodParameters.requiresOverrides && paramPermutations.Where(a => TypeManager.NormalizeType(a) == "any").Count() < paramPermutations.Count))
            {
                string thisPermutationAsString = string.Join(",", paramPermutations);

                if (!usedPermutations.Contains(thisPermutationAsString))
                {
                    MethodParameters permutationParams = methodParameters.CloneWithNewParamTypes(paramPermutations);

                    WriteMethod(tab, thisMethod.ShortDoc, thisMethod.Name, optionalFlag, permutationParams, thisType, thisMethod.Return, useExport, isStatic, methodWritten);
                    usedPermutations.Add(thisPermutationAsString);
                    methodWritten = true;
                }
            }
        }

        public void WriteMethod(string tab, string comment, string methodName, string optionalFlag, MethodParameters methodParameters, string returnType, Return returnMetadata, bool useExport, bool isStatic = false, bool isInterface = false, bool omitComment = false)
        {
            string exportString = useExport ? "export function " : "";
            string staticString = IsStaticMethod(isStatic, useExport, methodName) ? "static " : "";

            comment = string.Format("{0}/** [Method] {1}", tab, definitionWriter.FormatCommentText(comment));

            string paramsDoc = "";

            if (ShouldWriteMethod(isInterface, useExport, isStatic))
            {

                if (GeneretorConfiguration.Settings.InterfaceOnly)
                    returnType = TypeManager.ConvertToInterface(returnType);

                string methodOutput = string.Format("{0}{1}{2}(", staticString, methodName, optionalFlag);
                dynamic paramResult = AppendMethodParamOutput(tab, methodOutput, paramsDoc, methodParameters);

                writeMethodComment(tab, comment, paramResult.paramsDoc, returnMetadata, omitComment);
                WriteMethodDefinition(tab, paramResult.methodOutput, exportString, methodName, returnType);
            }
        }

        public void writeMethodComment(string tab, string comment, string paramsDoc, Return returnMetadata, bool omitComment)
        {
            string returnComment = null;
            if (returnMetadata != null)
                returnComment = string.Format("{0} * @returns {1} {2}", tab, returnMetadata.Type, definitionWriter.FormatCommentText(returnMetadata.Doc));

            if (ShouldIncludeComment(omitComment))
            {
                if (!string.IsNullOrEmpty(paramsDoc))
                {
                    definitionWriter.WriteToDefinition(comment);
                    definitionWriter.WriteToDefinition(paramsDoc);

                    if (!string.IsNullOrEmpty(returnComment))
                        definitionWriter.WriteToDefinition(returnComment);

                    definitionWriter.WriteToDefinition("{0} */", tab);
                }
                else
                {
                    if (!string.IsNullOrEmpty(returnComment))
                    {
                        definitionWriter.WriteToDefinition(comment);
                        definitionWriter.WriteToDefinition(returnComment);
                        definitionWriter.WriteToDefinition("{0} */", tab);
                    }
                    else
                    {
                        definitionWriter.WriteToDefinition("{0} */", comment);
                    }
                }
            }
        }

        public void WriteMethodDefinition(string tab, string methodOutput, string exportString, string methodName, string returnType)
        {
            if (methodName != "constructor")
            {
                definitionWriter.WriteToDefinition("{0}{1}{2}): {3};", tab, exportString, methodOutput, TypeManager.NormalizeType(returnType));
            }
            else
            {
                definitionWriter.WriteToDefinition("{0}{1});", tab, methodOutput);
            }
        }

        public dynamic AppendMethodParamOutput(string tab, string methodOutput, string paramsDoc, MethodParameters methodParameters)
        {

            for (int i = 0; i < methodParameters.paramNames.Count; i++)
            {
                Param thisParam = methodParameters.paramNames[i];
                string thisParamType = TypeManager.ConvertToInterface(methodParameters.paramTypes[i]);
                string thisParamName = thisParam.Name;

                paramsDoc += string.Format("{0} * @param {1} {2} {3}", tab, thisParamName, methodParameters.rawParamTypes[thisParamName], definitionWriter.FormatCommentText(thisParam.Short_Doc));

                if (!string.IsNullOrEmpty(thisParam.Doc) && Regex.IsMatch(thisParam.Doc, @"\boptional\b", RegexOptions.IgnoreCase))
                    thisParam.Optional = true;

                string spread = "";
                if (methodParameters.IsParamWithSpread(thisParam, thisParamType))
                    spread = "...";

                if (thisParamName == "class") thisParamName = "clazz";

                string optionalParamFlag = (thisParam.Optional || !string.IsNullOrEmpty(spread)) ? "?" : "";

                if (string.IsNullOrEmpty(spread) && GeneretorConfiguration.Settings.ForceAllParamsToOptional)
                    optionalParamFlag = "?";
                else if (methodParameters.HasParametersWithSpread())
                    optionalParamFlag = "";

                if (i > 0 && methodParameters.paramNames[i - 1].Optional && string.IsNullOrEmpty(spread))
                    optionalParamFlag = "?";

                methodOutput += string.Format("{0}{1}{2}:{3}", spread, thisParamName, optionalParamFlag, !string.IsNullOrEmpty(spread) ? "any[]" : TypeManager.NormalizeType(thisParamType));

                if (thisParam.Name == methodParameters.paramNames.Last().Name)
                    methodOutput += "";
                else
                {
                    methodOutput += ", ";
                    paramsDoc += "\n";
                }

            }

            return new { methodOutput = methodOutput, paramsDoc = paramsDoc };
        }

        public bool IsStaticMethod(bool isStatic, bool useExport, string methodName)
        {
            return isStatic && !useExport && methodName != "constructor";
        }

        public bool ShouldWriteMethod(bool isInterface, bool useExport, bool isStatic)
        {
            return !GeneretorConfiguration.Settings.InterfaceOnly || (GeneretorConfiguration.Settings.InterfaceOnly && (isInterface || useExport || isStatic));
        }

        public void WriteMethodAsProperty(Members thisMethod, string tabulacao, string optionalFlag)
        {
            definitionWriter.WriteToDefinition("{0}/** [Method] {1} */", tabulacao, definitionWriter.FormatCommentText(thisMethod.ShortDoc));
            definitionWriter.WriteToDefinition("{0} {1} {2}: any;", tabulacao, thisMethod.Name.Replace("-", ""), optionalFlag);
        }

        public void NormalizeMethodDoc(Members thisMethod)
        {
            if (string.IsNullOrEmpty(thisMethod.ShortDoc) && !string.IsNullOrEmpty(thisMethod.Short_Doc))
                thisMethod.ShortDoc = thisMethod.Short_Doc;
        }

        public void HandleReturnTypeSpecialCases(Members thisMethod, Component fileJson)
        {
            string propertyOverride = SpecialCases.getReturnTypeOverride(thisMethod.Return != null ? thisMethod.Return.Type : "");
            if (!string.IsNullOrEmpty(propertyOverride))
                thisMethod.Return.Type = propertyOverride;

            propertyOverride = SpecialCases.getReturnTypeOverride(fileJson.Name, thisMethod.Name);
            if ((thisMethod.Return != null || thisMethod.Return == null) && !string.IsNullOrEmpty(propertyOverride))
            {
                if (thisMethod.Return == null)
                {
                    var retorno = new Return() { Type = propertyOverride };
                    thisMethod.Return = retorno;
                }
                else
                    thisMethod.Return.Type = propertyOverride;
            }
        }

        public bool ShouldIncludeMethod(Component fileJson, Members thisMethod, bool isSingleton, bool isInterface, bool staticOnly, List<string> processedNames)
        {
            bool result = false;

            if (TypeManager.IsOwner(fileJson, thisMethod.Owner) || (fileJson.Mixins != null && fileJson.Mixins.Contains(thisMethod.Owner)) || isSingleton)
            {
                if ((!isInterface && (!isSingleton || (isSingleton && thisMethod.Name != "constructor"))) || (isInterface && thisMethod.Name != "constructor"))
                {
                    if ((GeneretorConfiguration.Settings.IncludePrivate == thisMethod.Private || GeneretorConfiguration.Settings.IncludePrivate) || thisMethod.Protected || thisMethod.Template)
                    {
                        //Novo
                        if (!staticOnly || (staticOnly && (thisMethod.Static || fileJson.Singleton || SpecialCases.shouldStatic(fileJson.Name, thisMethod.Name) || thisMethod.Name == "constructor")))
                        {
                            if (!processedNames.Contains(thisMethod.Name) && thisMethod.Deprecated == null && !SpecialCases.shouldRemoveMethod(fileJson.Name, thisMethod.Name))
                            {
                                if (!string.IsNullOrEmpty(thisMethod.Doc) || (!thisMethod.Doc.ToLower().Contains("overridden and disabled")))
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                }

                if (isInterface && thisMethod.Static)
                    result = false;

            }
            return result;
        }

        public bool ShouldIncludeComment(bool omitComment)
        {
            return !GeneretorConfiguration.Settings.OmitOverrideComments || (GeneretorConfiguration.Settings.OmitOverrideComments && !omitComment);
        }

        public bool ShouldCreateOverrideMethod(bool requiresOverrides, string[] tokenizedReturnTypes, string returnType)
        {
            return GeneretorConfiguration.Settings.UseFullTyping && requiresOverrides && tokenizedReturnTypes.First() == returnType;
        }
    }
}
