CREATE FUNCTION [dbo].[fn_GetLessonIdByLessonShortName]
(@lessonName NVARCHAR(1000))
RETURNS INT
AS
BEGIN

    DECLARE @id INT;

    SELECT @id = l.LessonId
    FROM dbo.LessonLessonShort AS l
    WHERE l.LessonName = @lessonName;

    RETURN @id;
END;
