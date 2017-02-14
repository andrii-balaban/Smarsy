CREATE PROCEDURE [dbo].[p_InsertAdsIfNotExists]
    @adDate DATETIME2(7),
    @adText NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Ad
    (
        [AdDate],
        [AdText]
    )
    SELECT @adDate,
        @adText
    WHERE NOT EXISTS
    (
        SELECT * FROM Ad WHERE AdDate = @adDate AND AdText = @adText
    );

END;