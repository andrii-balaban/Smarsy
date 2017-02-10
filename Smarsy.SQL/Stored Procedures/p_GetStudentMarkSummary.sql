CREATE PROCEDURE [dbo].[p_GetStudentMarkSummary]
    @studentId INT,
    @daysToShow INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT sm.StudentId,
        sm.LessonId,
        l.LessonName,
        sm.Mark,
        sm.MarkDate,
        sm.Reason,
		ROW_NUMBER() OVER(PARTITION BY l.LessonName ORDER BY sm.MarkDate ASC) AS RowNum
    FROM dbo.StudentMark AS sm
        JOIN dbo.Student AS s
            ON s.Id = sm.StudentId
        JOIN dbo.Lesson AS l
            ON l.Id = sm.LessonId
    WHERE s.Id = @studentId
          AND CAST(DATEADD(DAY, @daysToShow * -1, SYSDATETIME()) AS DATE) <= CAST(sm.CreateDtime AS DATE)
    ORDER BY l.LessonName,
        sm.MarkDate DESC;

END;