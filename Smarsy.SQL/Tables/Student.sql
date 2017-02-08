CREATE TABLE [dbo].[Student]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1), 
    [Name] NVARCHAR(100) NOT NULL, 
    [Login] VARCHAR(50) NOT NULL, 
    [Password] VARCHAR(50) NOT NULL, 
    [SmarsyChildId] INT NULL
)
