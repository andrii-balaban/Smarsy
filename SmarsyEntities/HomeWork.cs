namespace SmarsyEntities
{
    using System;

    public class HomeWork
    {
        public int LessonId { get; set; }

        public string LessonName { get; set; }

        public string HomeWorkDescr { get; set; }

        public DateTime HomeWorkDate { get; set; }

        public int TeacherId { get; set; }

        public string TeacherName { get; set; }
    }
}