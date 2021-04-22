ALTER PROCEDURE [dbo].[spGetUserWallets]
@userid nvarchar(450)
AS
BEGIN

--declare @acronim nvarchar(10)
--set @acronim = 'GPS'

SELECT w.[Id]
      ,w.[CurrencyId]
      ,w.[UserId]
      ,w.[Value]
      ,w.[WalletAddress]
	  ,c.Acronim
  FROM [Wallets] w
  left join [Currencies] c
  on w.CurrencyId = c.Id
  where w.[UserId] = @userid

END
