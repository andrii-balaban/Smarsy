namespace Smarsy
{
    using CommandLine;

    public class CommandLineOptions
    {
        [Option('m', "methods", Required = true, HelpText = "List of comma separated methods (actions) to be executed. ")]
        public string Methods { get; set; }

        [Option('t', "emailsTo", Required = true, HelpText = "List of comma separated emails for getting reports. ")]
        public string EmailsTo { get; set; }

        [Option('f', "emailFrom", Required = true, HelpText = "The gmail email used to send emails.")]
        public string EmailsFrom { get; set; }

        [Option('p', "emailPassword", Required = true, HelpText = "The password used to authenticate for sending emails.")]
        public string EmailPassword { get; set; }

        [Option('s', "smarsyLogin", Required = true, HelpText = "Login used for smarsy.")]
        public string SmarsyLogin { get; set; }
    }
}