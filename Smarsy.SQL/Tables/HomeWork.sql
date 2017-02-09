CREATE TABLE [dbo].[HomeWork]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [LessonId] INT NOT NULL,
    [HomeWork] NVARCHAR(2000) NOT NULL,
    [HomeWorkDate] DATE NOT NULL,
    [TeacherId] INT NULL, 
    CONSTRAINT [FK_HomeWork_Lesson]
        FOREIGN KEY ([LessonId])
        REFERENCES [Lesson] ([Id]), 
    CONSTRAINT [FK_HomeWork_Teacher] FOREIGN KEY ([TeacherId]) REFERENCES [Teacher]([Id]),
);

GO

CREATE TRIGGER [dbo].[tg_HomeWork_History]
ON [dbo].[HomeWork]
AFTER INSERT, DELETE, UPDATE
AS
BEGIN
    DECLARE @DCount INT,
        @ICount INT;

    -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT @DCount = COUNT(*)
    FROM deleted;
    SELECT @ICount = COUNT(*)
    FROM inserted;

    INSERT dbo.HomeWork_History
    (
        [Operation],
        [HomeWorkId],
        [LessonId],
        [HomeWork],
        [HomeWorkDate],
        [CreatedDtime]
    )
    SELECT Operation = CASE
                           WHEN I.Id = D.Id THEN
                               'M'
                           WHEN I.Id IS NOT NULL THEN
                               'A'
                           ELSE
                               'D'
                       END,
        COALESCE(I.Id, D.Id),
        COALESCE(I.LessonId, D.LessonId),
        COALESCE(I.HomeWork, D.HomeWork),
        COALESCE(I.HomeWorkDate, D.HomeWorkDate),
        SYSDATETIME()
    FROM inserted AS I
        FULL OUTER JOIN deleted AS D
            ON I.Id = D.Id;
END;