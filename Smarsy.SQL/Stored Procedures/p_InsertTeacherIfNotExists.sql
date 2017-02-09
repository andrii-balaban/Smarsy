CREATE PROCEDURE [dbo].[p_InsertTeacherIfNotExists]
    @teacherName NVARCHAR(100),
    @CreatedId INT = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Teacher
    (
        TeacherName
    )
    SELECT @teacherName
    WHERE NOT EXISTS
    (
        SELECT 1 FROM dbo.Teacher WHERE TeacherName = @teacherName
    );

    SELECT @CreatedId = Id
    FROM dbo.Teacher
    WHERE TeacherName = @teacherName;

END;