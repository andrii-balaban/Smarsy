CREATE FUNCTION [dbo].[GetStudentIdBySmarsyLogin] (@login INT)
RETURNS INT
AS
BEGIN

    DECLARE @studentId INT;

    SELECT @studentId = Id
    FROM dbo.Student AS s
    WHERE s.Login = @login;

    RETURN @studentId;
END;
