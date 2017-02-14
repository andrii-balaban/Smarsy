CREATE TABLE [dbo].[Student]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1), 
    [Name] NVARCHAR(100) NOT NULL, 
    [Login] VARCHAR(50) NULL, 
    [Password] VARCHAR(50) NULL, 
    [SmarsyChildId] INT NULL, 
    [BirthDate] DATETIME2 NOT NULL
)
