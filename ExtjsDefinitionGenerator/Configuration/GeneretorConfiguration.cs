namespace ExtjsDefinitionGenerator.Configuration
{
    using System.Configuration;

    public class GeneretorConfiguration : ConfigurationSection
    {
        private static GeneretorConfiguration settings = ConfigurationManager.GetSection("GeneretorConfiguration") as GeneretorConfiguration;

        public static GeneretorConfiguration Settings
        {
            get
            {
                return settings;
            }
        }

        [ConfigurationProperty("name", DefaultValue = "ExtJS")]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("version", DefaultValue = "4.2.1")]
        public string Version
        {
            get { return (string)this["version"]; }
            set { this["version"] = value; }
        }

        [ConfigurationProperty("charset", DefaultValue = "UTF-8")]
        public string Charset
        {
            get { return (string)this["charset"]; }
            set { this["charset"] = value; }
        }

        [ConfigurationProperty("includePrivate", DefaultValue = true)]
        public bool IncludePrivate
        {
            get { return (bool)this["includePrivate"]; }
            set { this["includePrivate"] = value; }
        }

        [ConfigurationProperty("interfaceOnly", DefaultValue = false)]
        public bool InterfaceOnly
        {
            get { return (bool)this["interfaceOnly"]; }
            set { this["interfaceOnly"] = value; }
        }

        [ConfigurationProperty("forceAllParamsToOptional", DefaultValue = true)]
        public bool ForceAllParamsToOptional
        {
            get { return (bool)this["forceAllParamsToOptional"]; }
            set { this["forceAllParamsToOptional"] = value; }
        }

        [ConfigurationProperty("useFullTyping", DefaultValue = true)]
        public bool UseFullTyping
        {
            get { return (bool)this["useFullTyping"]; }
            set { this["useFullTyping"] = value; }
        }

        [ConfigurationProperty("omitOverrideComments", DefaultValue = true)]
        public bool OmitOverrideComments
        {
            get { return (bool)this["omitOverrideComments"]; }
            set { this["omitOverrideComments"] = value; }
        }

        [ConfigurationProperty("customNamespaces", DefaultValue = "")]
        public string CustomNamespaces
        {
            get { return (string)this["customNamespaces"]; }
            set { this["customNamespaces"] = value; }
        }
    }
}
