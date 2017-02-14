CREATE PROCEDURE [dbo].[p_UpsertRemark]
    @remarkDate DATE,
    @lessonId INT,
    @remarkText NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Remark
    (
        RemarkDate,
        LessonId,
        RemarkText
    )
    SELECT @remarkDate,
        @lessonId,
        @remarkText
    WHERE NOT EXISTS
    (
        SELECT *
        FROM dbo.Remark
        WHERE RemarkDate = @remarkDate
              AND LessonId = @lessonId
              AND RemarkText = @remarkText
    );

END;