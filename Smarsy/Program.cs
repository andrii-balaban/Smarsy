using System.Configuration;
using System.Linq;
using System.Reflection;
using Smarsy.Logic;

namespace Smarsy
{
    using System;
    using CommandLine;

    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var options = new CommandLineOptions();

            //if (!Parser.Default.ParseArguments(args, options))
            //    return;

            var connectionString = ConfigurationManager.ConnectionStrings["SmarsyDbConnectionString"].ConnectionString;

            Smarsy smarsy = new Smarsy(new SmarsyRepository(connectionString), new SmarsyBrowser.SmarsyBrowser(), new DateTimeProvider());


            smarsy.LoginToSmarsy("test");

            InvokeMethods(smarsy, options);

            SendEmails(smarsy, options);
        }

        private static void SendEmails(Smarsy op, CommandLineOptions options)
        {
            string[] emails = options.GetEmails().ToArray();

            op.SendEmail(options.From, emails);
        }

        private static void InvokeMethods(Smarsy op, CommandLineOptions options)
        {
            string[] methodNames = options.GetMethods().ToArray();

            foreach (var methodName in methodNames)
            {
                InvokeMethodByName(op, methodName);
            }
        }

        private static void InvokeMethodByName(Smarsy op, string methodName)
        {
            Type classType = op.GetType();
            MethodInfo methodInfo = classType.GetMethod(methodName.Trim());

            if (methodInfo != null)
            {
                methodInfo.Invoke(op, null);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}