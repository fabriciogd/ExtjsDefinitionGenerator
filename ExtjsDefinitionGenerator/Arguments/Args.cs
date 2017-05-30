namespace ExtjsDefinitionGenerator.Arguments
{
    public class Args
    {
        private static Args _args = new Args();

        public static Args Current
        {
            get { return _args; }
        }

        /// <summary>
        /// -p | --path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// -op | --outpath
        /// </summary>
        public string OutPath { get; set; }
    }
}
