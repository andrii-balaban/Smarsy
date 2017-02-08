CREATE PROCEDURE [dbo].[p_GetStudentBySmarsyId] @smarsyChildId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT s.Id,
        s.Name,
        s.Login,
        s.Password,
        s.SmarsyChildId
    FROM dbo.Student AS s
    WHERE s.Login = @smarsyChildId;

END;