namespace SmarsyEntities
{
    using System;

    public class Student
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
    }
}