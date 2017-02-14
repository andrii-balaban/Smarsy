using System;

namespace SmarsyEntities
{
    public class Remark
    {
        public DateTime RemarkDate { get; set; }

        public string RemarkText { get; set; }

        public int LessonId { get; set; }

        public string LessonName { get; set; }
    }
}