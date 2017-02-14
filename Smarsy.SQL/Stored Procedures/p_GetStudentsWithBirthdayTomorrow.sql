CREATE PROCEDURE [dbo].[p_GetStudentsWithBirthdayTomorrow]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @tomorrowDate DATETIME2(7) = DATEADD(DAY, 1, SYSDATETIME());

    DECLARE @day INT = DAY(@tomorrowDate),
        @month INT = MONTH(@tomorrowDate);

    SELECT s.Id ,
           s.Name ,
           s.Login ,
           s.SmarsyChildId,
		   s.BirthDate
    FROM dbo.Student AS s
    WHERE DAY(BirthDate) = @day
          AND MONTH(BirthDate) = @month;

END;