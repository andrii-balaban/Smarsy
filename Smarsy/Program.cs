using System;

namespace Smarsy
{
    internal class Program
    {
        [STAThread]
        private static void Main()
        {
            var op = new Operational("90018970");
            op.InitStudentFromDb();
            op.LoginToSmarsy();
            op.UpdateMarks();
            op.UpdateHomeWork();
            op.SendEmail();
        }
    }
}

