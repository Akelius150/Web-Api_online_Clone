CREATE PROCEDURE [dbo].[GetUserIncomeWalletByAcronim]
@userid nvarchar(450),
@acronim nvarchar(10)
AS
BEGIN

SELECT iw.[Id]
      ,iw.[UserId]
      ,iw.[Address]
      ,iw.[AddressLabel]
      ,iw.[CurrencyAcronim]
      ,iw.[Created]
      ,iw.[LastUpdate]
  FROM [Exchange].[dbo].[IncomeWallets] iw
  where 
  iw.[UserId] = @userid
  AND iw.CurrencyAcronim = @acronim

END