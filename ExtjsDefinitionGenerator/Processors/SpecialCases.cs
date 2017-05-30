namespace ExtjsDefinitionGenerator.Processors
{
    using Models;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class SpecialCases
    {
        private static Dictionary<string, Dictionary<string, Dictionary<string, object>>> _specialCases;

        public static Dictionary<string, Dictionary<string, Dictionary<string, object>>> specialCases
        {
            get
            {
                if (_specialCases == null)
                {
                    _specialCases = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

                    CreateSpecialCases();
                }

                return _specialCases;
            }
        }

        private static Dictionary<string, Dictionary<string, Dictionary<string, object>>> CreateSpecialCases()
        {
            specialCases["removeProperty"] = new Dictionary<string, Dictionary<string, object>>();
            specialCases["removeMethod"] = new Dictionary<string, Dictionary<string, object>>();
            specialCases["methodToProperty"] = new Dictionary<string, Dictionary<string, object>>();
            specialCases["globalReturnTypeOverride"] = new Dictionary<string, Dictionary<string, object>>();
            specialCases["returnTypeOverride"] = new Dictionary<string, Dictionary<string, object>>();
            specialCases["rewriteMethod"] = new Dictionary<string, Dictionary<string, object>>();
            specialCases["propertyTypeOverride"] = new Dictionary<string, Dictionary<string, object>>();
            specialCases["static"] = new Dictionary<string, Dictionary<string, object>>();

            addRemovedMethod("Ext.dd.StatusProxy", "hide");
            addRemovedMethod("Ext.tip.Tip", "showAt");
            addRemovedMethod("Ext.tip.Tip", "showBy");
            addRemovedMethod("Ext.tip.ToolTip", "showAt");
            addRemovedMethod("Ext.tip.QuickTip", "showAt");
            addRemovedMethod("Ext.Base", "statics");
            addRemovedMethod("Ext.dom.Element", "select");
            addRemovedMethod("Ext.Component", "getBubbleTarget");
            addRemovedMethod("Ext.tip.ToolTip", "hide");
            addRemovedMethod("Ext.tip.QuickTip", "hide");
            addRemovedMethod("Ext.tip.ToolTip", "show");
            addRemovedMethod("Ext.tip.QuickTip", "show");
            addRemovedMethod("Ext.XTemplate", "compile");
            addRemovedMethod("Ext.data.proxy.Rest", "buildUrl");
            addRemovedMethod("Ext.dd.DDTarget", "getDragEl");
            addRemovedMethod("Ext.form.field.Base", "getInputId");
            addRemovedMethod("Ext.form.Field", "getInputId");
            addRemovedMethod("Ext.form.BaseField", "getInputId");
            addRemovedMethod("Ext.form.field.Base", "getSubTplMarkup");
            addRemovedMethod("Ext.form.Field", "getSubTplMarkup");
            addRemovedMethod("Ext.form.BaseField", "getSubTplMarkup");
            addRemovedMethod("Ext.form.field.Trigger", "getSubTplMarkup");
            addRemovedMethod("Ext.form.TriggerField", "getSubTplMarkup");
            addRemovedMethod("Ext.form.TwinTriggerField", "getSubTplMarkup");
            addRemovedMethod("Ext.form.Trigger", "getSubTplMarkup");
            addRemovedMethod("Ext.form.field.Spinner", "getSubTplMarkup");
            addRemovedMethod("Ext.form.Spinner", "getSubTplMarkup");


            addConvertMethodToProperty("Ext.AbstractComponent", "animate");
            addConvertMethodToProperty("Ext.util.Animate", "animate");
            addConvertMethodToProperty("Ext.form.field.Date", "safeParse");
            addConvertMethodToProperty("global", "_app");
            addConvertMethodToProperty("global", "_");

            addRemovedProperty("Ext.grid.column.Action", "isDisabled");
            addRemovedProperty("Ext.Component", "draggable");
            addRemovedProperty("Ext.ComponentLoader", "renderer");
            addRemovedProperty("Ext.data.TreeStore", "fields");
            addRemovedProperty("Ext.util.ComponentDragger", "delegate");
            addRemovedProperty("Ext.Class", "statics");

            addGlobalReturnTypeOverride("Ext.form.field.Field", "any");
            addGlobalReturnTypeOverride("Ext.slider.Multi", "any");
            addGlobalReturnTypeOverride("Ext.slider.Single", "any");
            addGlobalReturnTypeOverride("Ext.ModelManager", "any");
            addGlobalReturnTypeOverride("Ext.ComponentQuery", "any");

            addReturnTypeOverride("Ext.data.proxy.Rest", "buildUrl", "string");
            addReturnTypeOverride("Ext.data.AbstractStore", "load", "void");
            addReturnTypeOverride("Ext.dom.AbstractElement", "hide", "Ext.dom.Element");
            addReturnTypeOverride("Ext.dom.AbstractElement", "setVisible", "Ext.dom.Element");
            addReturnTypeOverride("Ext.dom.AbstractElement", "show", "Ext.dom.Element");
            addReturnTypeOverride("Ext.form.field.Text", "setValue", "any");
            addReturnTypeOverride("Ext.form.field.Base", "getRawValue", "any");
            addReturnTypeOverride("Ext.form.field.Field", "getName", "string");
            addReturnTypeOverride("Ext.form.field.Checkbox", "getSubmitValue", "any");
            addReturnTypeOverride("Ext.form.field.Base", "getSubmitValue", "any");
            addReturnTypeOverride("Ext", "setVersion", "any");
            addReturnTypeOverride("Ext.Version", "setVersion", "any");

            addReturnTypeOverride("Ext.form.field.Base", "getInputId", "string");
            addReturnTypeOverride("Ext.form.Field", "getInputId", "string");
            addReturnTypeOverride("Ext.form.BaseField", "getInputId", "string");

            addReturnTypeOverride("Ext.form.field.Base", "getSubTplMarkup", "string");
            addReturnTypeOverride("Ext.form.Field", "getSubTplMarkup", "string");
            addReturnTypeOverride("Ext.form.BaseField", "getSubTplMarkup", "string");
            addReturnTypeOverride("Ext.form.field.Trigger", "getSubTplMarkup", "string");
            addReturnTypeOverride("Ext.form.TriggerField", "getSubTplMarkup", "string");
            addReturnTypeOverride("Ext.form.TwinTriggerField", "getSubTplMarkup", "string");
            addReturnTypeOverride("Ext.form.Trigger", "getSubTplMarkup", "string");
            addReturnTypeOverride("Ext.form.field.Spinner", "getSubTplMarkup", "string");
            addReturnTypeOverride("Ext.form.Spinner", "getSubTplMarkup", "string");

            addStatic("Ext.dom.AbstractElement", "getActiveElement");
            addStatic("Ext.data.Connection", "on");

            addPropertyTypeOverride("Ext.ComponentLoader", "target", "HTMLElement | Ext.Element | string");

            return specialCases;
        }

        public static void addStatic(string className, string propertyName)
        {
            if (!specialCases["static"].ContainsKey(className))
                specialCases["static"].Add(className, new Dictionary<string, object>());

            specialCases["static"][className][propertyName] = true;
        }

        public static void addRemovedProperty(string className, string propertyName)
        {
            if (!specialCases["removeProperty"].ContainsKey(className))
                specialCases["removeProperty"].Add(className, new Dictionary<string, object>());

            specialCases["removeProperty"][className][propertyName] = true;
        }

        public static void addPropertyTypeOverride(string className, string propertyName, string newType = "any")
        {
            if (!specialCases["propertyTypeOverride"].ContainsKey(className))
                specialCases["propertyTypeOverride"].Add(className, new Dictionary<string, object>());

            specialCases["propertyTypeOverride"][className][propertyName] = newType;
        }

        public static void addRewriteMethod(string className, string methodName, string replacementJson)
        {
            if (!specialCases["rewriteMethod"].ContainsKey(className))
                specialCases["rewriteMethod"].Add(className, new Dictionary<string, object>());

            specialCases["rewriteMethod"][className][methodName] = replacementJson;
        }

        public static void addReturnTypeOverride(string className, string methodName, string newReturnType = "any")
        {
            if (!specialCases["returnTypeOverride"].ContainsKey(className))
                specialCases["returnTypeOverride"].Add(className, new Dictionary<string, object>());

            specialCases["returnTypeOverride"][className][methodName] = newReturnType;
        }

        public static void addGlobalReturnTypeOverride(string className, string newReturnType = "any")
        {
            if (!specialCases["globalReturnTypeOverride"].ContainsKey(className))
                specialCases["globalReturnTypeOverride"].Add(className, new Dictionary<string, object>());

            specialCases["globalReturnTypeOverride"][className][""] = newReturnType;
        }

        public static void addConvertMethodToProperty(string className, string methodName)
        {
            if (!specialCases["methodToProperty"].ContainsKey(className))
                specialCases["methodToProperty"].Add(className, new Dictionary<string, object>());

            specialCases["methodToProperty"][className][methodName] = true;
        }

        public static void addRemovedMethod(string className, string methodName)
        {
            if (!specialCases["removeMethod"].ContainsKey(className))
                specialCases["removeMethod"].Add(className, new Dictionary<string, object>());

            specialCases["removeMethod"][className][methodName] = true;
        }

        public static bool shouldRemoveProperty(string className, string propertyName)
        {
            return (specialCases["removeProperty"].ContainsKey(className) && (bool)specialCases["removeProperty"][className].ContainsKey(propertyName));
        }

        public static bool shouldStatic(string className, string methodName)
        {
            return (specialCases["static"].ContainsKey(className) && (bool)specialCases["static"][className].ContainsKey(methodName));
        }

        public static string getPropertyTypeOverride(string className, string propertyName = null)
        {
            if (propertyName != null && specialCases["propertyTypeOverride"].ContainsKey(className) && specialCases["propertyTypeOverride"][className].ContainsKey(propertyName))
                return (string)specialCases["propertyTypeOverride"][className][propertyName];
            return null;
        }

        public static bool shouldRewriteMethod(string className, string methodName)
        {
            return (specialCases["rewriteMethod"].ContainsKey(className) && (bool)specialCases["rewriteMethod"][className].ContainsKey(methodName));
        }

        public static bool shouldRemoveMethod(string className, string methodName)
        {
            return (specialCases["removeMethod"].ContainsKey(className) && (bool)specialCases["removeMethod"][className].ContainsKey(methodName));
        }

        public static bool shouldConvertToProperty(string className, string methodName)
        {
            return (specialCases["methodToProperty"].ContainsKey(className) && (bool)specialCases["methodToProperty"][className].ContainsKey(methodName));
        }

        public static Members getRewriteMethod(string className, string methodName, Members methodJson)
        {
            if (methodName != null && specialCases["rewriteMethod"].ContainsKey(className) && specialCases["rewriteMethod"][className].ContainsKey(methodName))
            {
                JsonConvert.PopulateObject((string)specialCases["rewriteMethod"][className][methodName], methodJson);
                return methodJson;
            }
            return null;
        }

        public static string getReturnTypeOverride(string className, string methodName = "")
        {
            if (specialCases["returnTypeOverride"].ContainsKey(className) && specialCases["returnTypeOverride"][className].ContainsKey(methodName))
                return (string)specialCases["returnTypeOverride"][className][methodName];
            if (specialCases["globalReturnTypeOverride"].ContainsKey(className) && specialCases["globalReturnTypeOverride"][className].ContainsKey(methodName))
                return (string)specialCases["globalReturnTypeOverride"][className][""];
            else
                return null;
        }
    }
}
