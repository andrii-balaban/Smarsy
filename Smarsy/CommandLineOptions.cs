using CommandLine;

namespace Smarsy
{
    public class CommandLineOptions
    {
        [Option('m', "methods", Required = true, HelpText = "List of comma separated methods (actions) to be executed. "
         )]
        public string Methods { get; set; }
    }
}