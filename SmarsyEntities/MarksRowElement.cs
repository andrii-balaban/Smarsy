using System.Collections.Generic;

namespace SmarsyEntities
{
    public class MarksRowElement
    {
        public List<StudentMark> Marks { get; set; }
        public string LessonName { get; set; }
        public int LessonId { get; set; }
    }
}