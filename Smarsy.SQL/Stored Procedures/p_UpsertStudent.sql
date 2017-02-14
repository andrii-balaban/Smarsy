CREATE PROCEDURE [dbo].[p_UpsertStudent]
    @studentName NVARCHAR(100),
    @birthDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Student
    (
        Name,
        BirthDate
    )
    SELECT @studentName,
        @birthDate
    WHERE NOT EXISTS
    (
        SELECT *
        FROM dbo.Student AS s
        WHERE Name = @studentName
              AND BirthDate = @birthDate
    );

END;