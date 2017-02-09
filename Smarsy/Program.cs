using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Smarsy.Logic;
using SmarsyEntities;

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

