namespace ExtjsDefinitionGenerator.Models
{
    using Converters;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class Component
    {
        public Component()
        {
            this.Singleton = false;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("doc")]
        public string Doc { get; set; }

        [JsonProperty("extends")]
        public string Extends { get; set; }

        [JsonProperty("mixins")]
        public IList<string> Mixins { get; set; }

        [JsonProperty("alternateClassNames")]
        public IList<string> AlternateClassNames { get; set; }

        [JsonProperty("singleton")]
        [JsonConverter(typeof(BoolConverter))]
        public bool Singleton { get; set; }

        [JsonProperty("members")]
        public IList<Members> Members { get; set; }

        [JsonProperty("superclasses")]
        public IList<string> Superclasses { get; set; }
    }

    public class Members
    {
        [JsonProperty("tagname")]
        public string Tagname { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("doc")]
        public string Doc { get; set; }

        [JsonProperty("shortDoc")]
        public string ShortDoc { get; set; }

        [JsonProperty("short_doc")]
        public string Short_Doc { get; set; }

        [JsonProperty("optional")]
        public bool Optional { get; set; }

        [JsonProperty("private")]
        [JsonConverter(typeof(BoolConverter))]
        public bool Private { get; set; }

        [JsonProperty("static")]
        [JsonConverter(typeof(BoolConverter))]
        public bool Static { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("return")]
        public Return Return { get; set; }

        [JsonProperty("protected")]
        [JsonConverter(typeof(BoolConverter))]
        public bool Protected { get; set; }

        [JsonProperty("template")]
        [JsonConverter(typeof(BoolConverter))]
        public bool Template { get; set; }

        [JsonProperty("deprecated")]
        public Deprecated Deprecated { get; set; }

        [JsonProperty("params")]
        public IList<Param> Params { get; set; }

        [JsonProperty("autodetected")]
        public Autodetected Autodetected { get; set; }
    }

    public class Autodetected
    {
        [JsonProperty("static")]
        [JsonConverter(typeof(BoolConverter))]
        public bool Static { get; set; }
    }

    public class Deprecated
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class Param
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("doc")]
        public string Doc { get; set; }

        [JsonProperty("short_doc")]
        public string Short_Doc { get; set; }

        [JsonProperty("optional")]
        public bool Optional { get; set; }

        [JsonProperty("default")]
        public string Default { get; set; }

        [JsonProperty("html_type")]
        public string HtmlType { get; set; }

        [JsonProperty("tagname")]
        public string Tagname { get; set; }
    }


    public class Return
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("doc")]
        public string Doc { get; set; }

        [JsonProperty("html_type")]
        public string HtmlType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Method
    {

        [JsonProperty("tagname")]
        public string Tagname { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("doc")]
        public string Doc { get; set; }

        [JsonProperty("params")]
        public IList<Param> Params { get; set; }

        [JsonProperty("return")]
        public Return Return { get; set; }

        [JsonProperty("throws")]
        public object Throws { get; set; }

        [JsonProperty("inheritable")]
        public object Inheritable { get; set; }

        [JsonProperty("private")]
        public bool? Private { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("shortDoc")]
        public string ShortDoc { get; set; }

        [JsonProperty("autodetected")]
        public bool? Autodetected { get; set; }
    }
}
