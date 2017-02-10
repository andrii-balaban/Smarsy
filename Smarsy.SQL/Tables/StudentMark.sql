CREATE TABLE [dbo].[StudentMark]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1), 
    [StudentId] INT NOT NULL, 
    [LessonId] INT NOT NULL, 
    [Mark] INT NOT NULL, 
    [MarkDate] DATE NOT NULL, 
    [Reason] NVARCHAR(1000) NULL, 
    [CreateDtime] DATETIME2 NULL DEFAULT SYSDATETIME(), 
    CONSTRAINT [FK_StudentMark_Student] FOREIGN KEY ([StudentId]) REFERENCES [Student]([Id]),
    CONSTRAINT [FK_StudentMark_Lesson] FOREIGN KEY ([LessonId]) REFERENCES [Lesson]([Id])
)
