CREATE TABLE [dbo].[HomeWork_History]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[Operation] CHAR(1),
    [HomeWorkId] INT,
    [LessonId] INT,
    [HomeWork] NVARCHAR(2000),
    [HomeWorkDate] DATE,
    [CreatedDtime] DATETIME2(7)
);
