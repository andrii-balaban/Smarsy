CREATE PROCEDURE [dbo].[p_GetNewRemarks]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Id,
        r.RemarkDate,
        r.LessonId,
		l.LessonName,
        r.RemarkText,
        r.CreateDtime
    FROM dbo.Remark AS r
	LEFT JOIN dbo.Lesson AS l ON l.Id = r.LessonId
    WHERE CAST(r.CreateDtime AS DATE) >= CAST(DATEADD(DAY, -1, SYSDATETIME()) AS DATE);
END;