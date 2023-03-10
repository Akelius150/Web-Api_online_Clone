USE [Exchange]
GO
/****** Object:  StoredProcedure [dbo].[SendCoins]    Script Date: 12/16/2021 11:25:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SendCoins] 
@senderUserId nvarchar(450),
@receiverUserId nvarchar(450),
@typeSend int,
@typeRecieve int,

@Comment nvarchar(450),
@currencyAcronim nvarchar(10),
@value decimal(38,20),

@senderWalletId int,
@receiverWalletId int,
@hash varchar(66)

AS

declare @senderValue decimal(38,20)
declare @receiverValue decimal(38,20) 

BEGIN

SELECT @senderValue = Value From Wallets WHERE ID = @senderWalletId
SELECT @receiverValue = Value From Wallets WHERE ID = @receiverWalletId

UPDATE [Exchange].[dbo].[Wallets]
SET Value -= @value, LastUpdate = GETDATE()
WHERE Id = @senderWalletId

UPDATE [Exchange].[dbo].[Wallets]
SET Value += @value, LastUpdate = GETDATE()
WHERE Id = @receiverWalletId

insert into [Exchange].[dbo].[Transfers] (WalletFromId, WalletToId, Value, Date, CurrencyAcronim, Hash, Comment)
values (@senderWalletId, @receiverWalletId, @value, GETDATE(), @currencyAcronim, @hash, @Comment)

insert into [Exchange].[dbo].[Events] (UserId, Type, Value, 
StartBalance, ResultBalance, Comment, WhenDate, CurrencyAcronim)
values (@senderUserId, @typeSend, @value, @senderValue, @senderValue - @value,
@Comment, GETDATE(), @currencyAcronim)

insert into [Exchange].[dbo].[Events] (UserId, Type, Value,
StartBalance, ResultBalance, Comment, WhenDate, CurrencyAcronim)
values (@receiverUserId, @typeRecieve, @value, @receiverValue, @receiverValue + @value,
@Comment, GETDATE(), @currencyAcronim)

END

