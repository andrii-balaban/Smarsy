CREATE FUNCTION [dbo].[fn_GetLessonIdByLessonName] (@lessonName NVARCHAR(1000))
RETURNS INT
AS
BEGIN

    DECLARE @id INT;

    SELECT @id = l.Id
    FROM dbo.Lesson AS l
    WHERE l.LessonName = @lessonName;

    RETURN @id;
END;
