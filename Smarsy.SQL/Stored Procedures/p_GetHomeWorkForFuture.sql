CREATE PROCEDURE [dbo].[p_GetHomeWorkForFuture]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT hw.HomeWorkDate,
        l.LessonName,
        t.TeacherName,
        hw.HomeWork
    FROM dbo.HomeWork AS hw
        JOIN dbo.Lesson AS l
            ON l.Id = hw.LessonId
        JOIN dbo.Teacher AS t
            ON t.Id = hw.TeacherId
    WHERE hw.HomeWorkDate > SYSDATETIME()
    ORDER BY 1,
        2,
        3;
END;