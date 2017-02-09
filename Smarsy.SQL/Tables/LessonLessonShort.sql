CREATE TABLE [dbo].[LessonLessonShort]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[LessonId] INT NOT NULL, 
    [LessonName] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [FK_LessonLessonShort_Lesson] FOREIGN KEY ([LessonId]) REFERENCES [Lesson]([Id])
)
