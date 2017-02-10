CREATE PROCEDURE [dbo].[p_UpsertStudentMarksByLesson]
    @studentId INT,
    @lessonId INT,
    @marksWithDates dbo.udt_MarksWithDates READONLY
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewMarks dbo.udt_MarksWithDates;

    INSERT INTO @NewMarks
    SELECT DISTINCT
        mwd.Mark,
        mwd.MarkDate,
		mwd.Reason
    FROM @marksWithDates mwd
        LEFT JOIN dbo.StudentMark AS sm
            ON sm.StudentId = @studentId
               AND sm.LessonId = @lessonId
               AND sm.Mark = mwd.Mark
               AND sm.MarkDate = mwd.MarkDate
    WHERE sm.StudentId IS NULL;

    INSERT INTO dbo.StudentMark
    (
        StudentId,
        LessonId,
        Mark,
        MarkDate,
        Reason
    )
    SELECT @studentId,
        @lessonId,
        Mark,
        MarkDate,
        Reason
    FROM @NewMarks;

END;