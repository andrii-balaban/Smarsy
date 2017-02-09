CREATE PROCEDURE [dbo].[p_UpsertHomeWork]
    @lessonId INT,
    @homeWork NVARCHAR(2000),
    @homeWorkDate DATE,
	@teacherId INT NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.HomeWork
    (
        LessonId,
        HomeWork,
        HomeWorkDate,
		TeacherId
    )
    SELECT @lessonId,
        @homeWork,
        @homeWorkDate,
		@teacherId
    WHERE NOT EXISTS
    (
        SELECT *
        FROM dbo.HomeWork AS hw
        WHERE hw.LessonId = @lessonId
              AND hw.HomeWorkDate = @homeWorkDate
			  AND COALESCE(hw.TeacherId, -1) = COALESCE(@teacherId, -1)
    );

    IF (@@ROWCOUNT = 0)
    BEGIN
        UPDATE dbo.HomeWork
        SET HomeWork = @homeWork
        WHERE LessonId = @lessonId
              AND HomeWorkDate = @homeWorkDate
              AND COALESCE(HomeWork, '') <> COALESCE(@homeWork, '')
			  AND COALESCE(TeacherId, -1) = COALESCE(@teacherId, -1);
    END;

END;