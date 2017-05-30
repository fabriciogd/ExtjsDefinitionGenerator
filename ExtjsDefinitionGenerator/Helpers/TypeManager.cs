namespace ExtjsDefinitionGenerator.Helpers
{
    using ExtjsDefinitionGenerator.Configuration;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class TypeManager
    {
        public static bool IsCustomNamespace(string name)
        {
            string[] customNamespaces = GeneretorConfiguration.Settings.CustomNamespaces.Split(',');

            return customNamespaces.Any(a => name.Contains(a));
        }

        public static string GetModule(string name)
        {
            var parts = name.Split('.').ToList();

            if (parts.Count() == 1)
                return name;

            parts.Remove(parts.Last());

            for (int i = 0; i < parts.Count(); i++)
            {
                if (parts[i] == "ext")
                    parts[i] = "Ext";
            }

            return string.Join(".", parts);
        }

        public static string GetClassName(string name)
        {
            var parts = name.Split('.').ToList();

            return parts.Last();
        }

        public static string GetExtends(Component jsonFile, bool isInterface)
        {
            string result = string.Empty;

            if (jsonFile.Name == "Ext.Base")
            {
                jsonFile.Superclasses = new List<string>() { "Ext.Class" };
            }
            else if (jsonFile.Superclasses == null && !string.IsNullOrEmpty(jsonFile.Extends))
            {
                jsonFile.Superclasses = new List<string>() { jsonFile.Extends };
            }

            if (jsonFile.Superclasses != null && jsonFile.Superclasses.Count > 0)
            {
                if (isInterface)
                {
                    if (jsonFile.Superclasses.Last().Contains("Ext.") && jsonFile.Superclasses.Last() != jsonFile.Name)
                        result = string.Format("extends {0}", NormalizeType(ConvertToInterface(jsonFile.Superclasses.Last()), true));
                }
                else if (GetClassName(jsonFile.Name) != jsonFile.Superclasses.Last())
                    result = string.Format("extends {0}", NormalizeType(jsonFile.Superclasses.Last(), true));
            }

            if (isInterface)
            {
                string interfaces = GetImplementedInterfaces(jsonFile, isInterface);
                if (string.IsNullOrEmpty(result) && interfaces.Count() > 0)
                {
                    result = "extends ";
                }
                else if (!string.IsNullOrEmpty(result) && interfaces.Count() > 0)
                {
                    result += ", ";
                }
                if (interfaces.Count() > 0)
                    result += interfaces;
            }

            return result;
        }

        public static string GetImplementedInterfaces(Component jsonFile, bool isInterface)
        {
            string result = "";

            if (!isInterface)
                result = NormalizeType(ConvertToInterface(jsonFile.Name), true);

            List<string> implementedInterfaces = new List<string>();

            if (jsonFile.Mixins != null)
            {
                foreach (var mixin in jsonFile.Mixins)
                {
                    implementedInterfaces.Add(NormalizeType(ConvertToInterface(mixin), true));
                }
            }

            if (implementedInterfaces.Count > 0)
            {
                if (!isInterface)
                    result += ", ";
                result += string.Join(", ", implementedInterfaces);
            }

            return result;
        }

        public static string NormalizeType(string typeName = "any", bool forceFullType = false)
        {
            if ((!GeneretorConfiguration.Settings.UseFullTyping && !forceFullType) ||
                typeName == "any" ||
                typeName == "" ||
                typeName == null ||
                Regex.IsMatch(typeName, "\".*?\""))
            {
                return "any";
            }

            typeName = typeName.Replace("\\.\\.\\.", "").Trim();

            if (typeName.Contains("Ext.") || typeName.Contains("ext.") || IsCustomNamespace(typeName))
                typeName = string.Format("{0}.{1}", GetModule(typeName), GetClassName(typeName));

            string capitalizedTypeName = typeName;

            // TypeScript gets confused between native Function type and Ext.Function class...
            if (capitalizedTypeName == "Function") typeName = "any";
            if (capitalizedTypeName == "Mixed") typeName = "any";
            if (capitalizedTypeName == "TextNode") typeName = "any";
            if (capitalizedTypeName == "Arguments") typeName = "any";
            if (capitalizedTypeName == "Object") typeName = "any";
            if (capitalizedTypeName == "object") typeName = "any";
            if (capitalizedTypeName == "String") typeName = "string";
            if (capitalizedTypeName == "SINGLE") typeName = "string";
            if (capitalizedTypeName == "Boolean") typeName = "boolean";
            if (capitalizedTypeName == "Bool") typeName = "boolean";
            if (capitalizedTypeName == "Integer") typeName = "number"; //Documentação errada no extjs
            if (capitalizedTypeName == "Number") typeName = "number";
            if (capitalizedTypeName == "Date") typeName = "any";
            if (capitalizedTypeName == "*") typeName = "any";
            if (capitalizedTypeName == "Null") typeName = "any";
            if (capitalizedTypeName == "Undefined") typeName = "any";
            if (capitalizedTypeName == "Htmlelement") typeName = "HTMLElement";
            if (capitalizedTypeName == "Ext.data.INodeinterface") typeName = "Ext.data.INodeInterface";
            if (capitalizedTypeName == "Ext.dom.ICompositeelementlite") typeName = "Ext.dom.ICompositeElementLite";
            if (capitalizedTypeName == "Google.maps.Map") typeName = "any";
            if (capitalizedTypeName == "FileTransfer") typeName = "any";
            if (capitalizedTypeName == "Ext.plugin.IPlugin") typeName = "Ext.IBase";

            if (capitalizedTypeName == "Array") typeName = "any[]";
            if (capitalizedTypeName == "array") typeName = "any[]";
            if (capitalizedTypeName == "Arrary") typeName = "any[]";
            if (capitalizedTypeName == "String[]") typeName = "string[]";
            if (capitalizedTypeName == "Boolean[]") typeName = "boolean[]";
            if (capitalizedTypeName == "Number[]") typeName = "number[]";
            if (capitalizedTypeName == "Date[]") typeName = "any";
            if (capitalizedTypeName == "Object[]") typeName = "any[]";
            if (capitalizedTypeName == "Mixed[]") typeName = "any[]";
            if (capitalizedTypeName == "Htmlelement[]") typeName = "HTMLElement[]";

            if (capitalizedTypeName == "Int") typeName = "number";
            if (capitalizedTypeName == "XMLElement") typeName = "any";

            return typeName;
        }

        public static string ConvertToInterface(string type)
        {
            string result = type;
            if (type.Contains("Ext.") || type.Contains("ext.") || IsCustomNamespace(type))
            {
                result = string.Format("{0}.I{1}", GetModule(type), GetClassName(type));
            }

            return result;
        }

        public static string NormalizeReturnType(string typeName = "any", bool forceFullType = false)
        {
            if (typeName.Contains("/") || typeName.Contains("|"))
            {
                if (typeName == "Ext.dom.CompositeElementLite/Ext.dom.CompositeElement") typeName = "Ext.dom.CompositeElementLite";
                if (typeName == "Date/null") typeName = "any";
            }
            if (typeName.Contains("undefined")) typeName = typeName.Replace("undefined", "void");
            return typeName;
        }


        public static string[] GetTokenizedTypes(string types)
        {
            if (string.IsNullOrEmpty(types)) types = "void";
            string[] result = types.Split(new char[] { '/', '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length > 1)
                result = new string[] { "any" };
            return result; ;
        }


        public static string[] getTokenizedReturnTypes(Return @return)
        {
            if (@return == null)
                @return = new Return() { Type = "" };
            var types = NormalizeReturnType(@return.Type);
            return GetTokenizedTypes(types);
        }

        public static bool IsOwner(Component jsonFile, string candidate)
        {
            bool result = false;

            if (jsonFile.Name == candidate)
            {
                result = true;
            }
            else if (jsonFile.AlternateClassNames.Count > 0)
            {
                foreach (var thisClassName in jsonFile.AlternateClassNames)
                {
                    if (thisClassName == candidate)
                        result = true;
                }
            }

            return result;
        }

    }
}
