CREATE TABLE [dbo].[Remark]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [RemarkDate] DATE NOT NULL, 
    [LessonId] INT NOT NULL, 
    [RemarkText] NVARCHAR(MAX) NOT NULL, 
    [CreateDtime] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
    CONSTRAINT [FK_Remark_Lesson] FOREIGN KEY ([LessonId]) REFERENCES [Lesson]([Id])
)
