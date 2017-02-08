namespace SmarsyEntities
{
    public class Student
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public int StudentId { get; set; }
        public string Name { get; set; }
        public int SmarsyChildId { get; set; }
        public Student()
        {
            
        }

        public Student(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}