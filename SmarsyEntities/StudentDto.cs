using System.Windows.Forms;

namespace SmarsyEntities
{
    using System;

    public class StudentDto
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public int StudentId { get; set; }

        public string Name { get; set; }

        public int SmarsyChildId { get; set; }

        public DateTime BirthDate { get; set; }
    }

    public class SmarsyStudent : SmarsyElement
    {
        private readonly StudentDto _studentDto;

        public SmarsyStudent()
        {
            _studentDto = new StudentDto();
        }

        public SmarsyStudent(StudentDto student)
        {
            _studentDto = student;
            Credentials = new SmarsyCredentials(student.Login, student.Password);
        }
        
        public int StudentId => _studentDto.StudentId;

        public string Name
        {
            get => _studentDto.Name;
            set => _studentDto.Name = value;
        }

        public int SmarsyChildId => _studentDto.SmarsyChildId;

        public DateTime BirthDate
        {
            get => _studentDto.BirthDate;
            set => _studentDto.BirthDate = value;
        }

        public SmarsyCredentials Credentials { get; }

        public StudentDto ToDto()
        {
            return _studentDto;
        }

        public override void ParseFromHtmlElement(HtmlElement row)
        {
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
                    Name = studentRow.InnerHtml;
                }

                if (i == 2)
                {
                    birthDate = studentRow.InnerHtml;
                }

                if (i == 3)
                {
                    BirthDate = TextProcessor.Processor.GetDateFromText(birthDate, int.Parse(studentRow.InnerHtml));
                }

                i++;
            }
        }
    }
}