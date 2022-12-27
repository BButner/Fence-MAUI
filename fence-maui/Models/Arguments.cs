using CommandLine;

namespace fence_maui.Models
{
    public class Arguments
    {
        [Option( 'c', "config", Required = false, HelpText = "Absolute path to the Config JSON file" )]
        public string Config { get; set; }
    }
}