using System.Windows.Forms;

namespace SmarsyEntities
{
    using System;

    public class Student : SmarsyElement<Student>
    {
        public Student()
        {
        }

        public Student(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get; set; }

        public string Password { get; set; }

        public int StudentId { get; set; }

        public string Name { get; set; }

        public int SmarsyChildId { get; set; }

        public DateTime BirthDate { get; set; }
        public override Student GetElement(HtmlElement row)
        {
            var student = new Student();
            var i = 0;
            var birthDate = string.Empty;

            foreach (HtmlElement studentRow in row.GetElementsByTagName("td"))
            {
                if (i == 0)
                {
                    i++;
                    continue; // skip student sequence number
                }

                if (i == 1)
                {
                    student.Name = studentRow.InnerHtml;
                }

                if (i == 2)
                {
                    birthDate = studentRow.InnerHtml;
                }

                if (i == 3)
                {
                    student.BirthDate = TextProcessor.GetDateFromText(birthDate, int.Parse(studentRow.InnerHtml));
                }

                i++;
            }

            return student;
        }
    }
}