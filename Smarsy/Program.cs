using System.Configuration;
using System.Linq;
using System.Reflection;
using Smarsy.Logic;
using SmarsyEntities;

namespace Smarsy
{
    using System;
    using CommandLine;

    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            if (TryGetOptionsFromArguments(args, out var options))
                return;

            Smarsy smarsy = CreateSmarsy();

            LoginToSmarsy(smarsy, "test");

            InvokeMethods(smarsy, options);

            SendEmails(smarsy, options);
        }

        private static bool TryGetOptionsFromArguments(string[] args, out CommandLineOptions options)
        {
            options = new CommandLineOptions();

            if (!Parser.Default.ParseArguments(args, options))
                return true;

            return false;
        }

        private static void LoginToSmarsy(Smarsy smarsy, string userLogin)
        {
            smarsy.Login(userLogin);
        }

        private static Smarsy CreateSmarsy()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SmarsyDbConnectionString"].ConnectionString;

            return new Smarsy(new SmarsyRepository(connectionString), new SmarsyBrowser.SmarsyBrowser(), new DateTimeProvider());
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