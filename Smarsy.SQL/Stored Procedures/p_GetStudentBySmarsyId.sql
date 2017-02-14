CREATE PROCEDURE [dbo].[p_GetStudentBySmarsyId] @login VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT s.Id,
        s.Name,
        s.Login,
        s.Password,
        s.SmarsyChildId,
		s.BirthDate
    FROM dbo.Student AS s
    WHERE s.Login = @login;

END;