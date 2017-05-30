namespace ExtjsDefinitionGenerator.Processors
{
    using Configuration;
    using Helpers;
    using Models;
    using System.Collections.Generic;
    using System.Linq;

    public class MethodParameters
    {
        public Members method;
        public bool requiresOverrides = false;
        public Dictionary<string, string> rawParamTypes = new Dictionary<string, string>();
        public List<Param> paramNames = new List<Param>();
        public List<string> paramTypes = new List<string>();

        public MethodParameters(Members method)
        {
            this.method = method;
            this.paramNames = this.method.Params.ToList();

            foreach (var thisParam in this.paramNames)
            {
                if (GeneretorConfiguration.Settings.UseFullTyping)
                {
                    string[] tokenizedParamTypes = TypeManager.GetTokenizedTypes(thisParam.Type);
                    if (tokenizedParamTypes.Length > 1 && !requiresOverrides)
                    {
                        requiresOverrides = true;
                    }

                    if (this.IsParamWithSpread(thisParam, thisParam.Type) && tokenizedParamTypes.Length == 1)
                        tokenizedParamTypes[0] = "..." + tokenizedParamTypes[0].Replace("...", "");

                    paramTypes.AddRange(tokenizedParamTypes);
                }
                else
                {
                    paramTypes.Add(thisParam.Type);
                }

                rawParamTypes[thisParam.Name] = thisParam.Type;
            }
        }

        public MethodParameters CloneWithNewParamTypes(List<string> newParamTypes)
        {
            MethodParameters methodParameters = new MethodParameters(this.method);
            methodParameters.paramNames = paramNames;
            methodParameters.paramTypes = newParamTypes;
            methodParameters.rawParamTypes = rawParamTypes;
            methodParameters.requiresOverrides = requiresOverrides;
            return methodParameters;
        }

        public bool HasParametersWithSpread()
        {
            return paramTypes.Any(a => a.StartsWith("...") || a.EndsWith("..."));
        }

        public bool IsParamWithSpread(Param thisParam, string thisParamType)
        {
            return this.paramNames.Last().Name == thisParam.Name && (thisParamType.StartsWith("...") || thisParamType.EndsWith("..."));
        }

        public bool HasOnlyOneSignature()
        {
            return !GeneretorConfiguration.Settings.UseFullTyping || paramNames.Count() >= 0;
        }
    }
}
