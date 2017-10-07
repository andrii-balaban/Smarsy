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

            if (!Parser.Default.ParseArguments(args, options))
                return;

            SmarsyOperations smarsyOperations = new SmarsyOperations(new SqlServerLogic(options.SmarsyLogin), new SmarsyBrowser.SmarsyBrowser(), new DateTimeProvider());

            //// options.Methods = "LoginToSmarsy,UpdateMarks,UpdateHomeWork,UpdateAds,UpdateStudents,UpdateRemarks";
            options.Methods = "LoginToSmarsy,UpdateRemarks";

            InvokeMethods(smarsyOperations, options);

            SendEmails(smarsyOperations, options);
        }

        private static void SendEmails(SmarsyOperations op, CommandLineOptions options)
        {
            string[] emails = options.GetEmails().ToArray();

            op.SendEmail(options.From, emails);
        }

        private static void InvokeMethods(SmarsyOperations op, CommandLineOptions options)
        {
            string[] methodNames = options.GetMethods().ToArray();

            foreach (var methodName in methodNames)
            {
                InvokeMethodByName(op, methodName);
            }
        }

        private static void InvokeMethodByName(SmarsyOperations op, string methodName)
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