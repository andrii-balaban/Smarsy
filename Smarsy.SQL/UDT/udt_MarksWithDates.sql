CREATE TYPE [dbo].[udt_MarksWithDates] AS TABLE
(
    Mark INT,
    MarkDate DATE,
	Reason NVARCHAR(1000)
);
