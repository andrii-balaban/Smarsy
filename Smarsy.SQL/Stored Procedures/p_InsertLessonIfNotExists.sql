CREATE PROCEDURE [dbo].[p_InsertLessonIfNotExists] @lessonName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Lesson
    (
        LessonName
    )
    SELECT @lessonName
    WHERE NOT EXISTS
    (
        SELECT 1 FROM dbo.Lesson WHERE LessonName = @lessonName
    );

END;