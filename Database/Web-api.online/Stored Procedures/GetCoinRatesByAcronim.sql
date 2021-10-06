ALTER PROCEDURE [dbo].[GetCoinRatesByAcronim]
@acronim nvarchar(10)
AS
BEGIN

SELECT * 
FROM [web-api.online].[dbo].[CoinsRates] 
WHERE Acronim = @acronim

END
