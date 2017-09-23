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

            Operational op = new Operational(new SqlServerLogic(options.SmarsyLogin), options.SmarsyLogin);
            op.InitStudentFromDb();

            //// options.Methods = "LoginToSmarsy,UpdateMarks,UpdateHomeWork,UpdateAds,UpdateStudents,UpdateRemarks";
            options.Methods = "LoginToSmarsy,UpdateRemarks";

            InvokeMethods(options, op);

            SendEmails(options, op);
        }

        private static void SendEmails(CommandLineOptions options, Operational op)
        {
            string[] emailToList = options.EmailsTo.Split(',');

            op.SendEmail(emailToList, options.EmailsFrom, options.EmailPassword);
        }

        private static void InvokeMethods(CommandLineOptions options, Operational op)
        {
            string[] methodNames = GetMethodNames(options);

            foreach (var methodName in methodNames)
            {
                InvokeMethodByName(op, methodName);
            }
        }

        private static string[] GetMethodNames(CommandLineOptions options)
        {
            string[] methodNames = options.Methods.Split(',');
            return methodNames;
        }

        private static void InvokeMethodByName(Operational op, string methodName)
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