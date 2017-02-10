using System;

namespace Smarsy
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            // -m "UpdateMarks,UpdateHomeWork,SendEmail"
            var options = new CommandLineOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                var op = new Operational("90018970");
                op.InitStudentFromDb();

                var methods = options.Methods.Split(',');
                foreach (var method in methods)
                {
                    var classType = op.GetType();
                    var methodInfo = classType.GetMethod(method);
                    if (methodInfo != null) methodInfo.Invoke(op, null);
                    else throw new NotImplementedException();
                }
            }
        }
    }
}

