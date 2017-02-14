namespace SmarsyEntities
{
    using System.Collections.Generic;

    public class LessonMark
    {
        public List<StudentMark> Marks { get; set; }

        public string LessonName { get; set; }

        public int LessonId { get; set; }
    }
}