CREATE FUNCTION [dbo].[fn_GetStudentIdByChildId] (@childId INT)
RETURNS INT
AS
BEGIN

    DECLARE @studentId INT;

    SELECT @studentId = Id
    FROM dbo.Student AS s
    WHERE s.SmarsyChildId = @childId;

    RETURN @studentId;
END;
