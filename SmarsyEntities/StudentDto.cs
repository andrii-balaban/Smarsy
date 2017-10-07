using System.Security;

namespace SmarsyEntities
{
    using System;

    public class StudentDto
    {
        public string Login { get; set; }

        public SecureString Password { get; set; }

        public int StudentId { get; set; }

        public string Name { get; set; }

        public int SmarsyChildId { get; set; }

        public DateTime BirthDate { get; set; }
    }
}