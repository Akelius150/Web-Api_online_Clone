USE [Exchange]
GO
/****** Object:  StoredProcedure [dbo].[spCreateEvent]    Script Date: 28.08.2021 12:53:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[spSendCoins]
@senderUserId nvarchar(450),
@receiverUserId nvarchar(450),
@typeSend int,
@typeRecieve int,
@senderCommentEvent nvarchar(450),
@receiverCommentEvent nvarchar(450),

@currencyAcronim nvarchar(10),
@value decimal(38,20),

@senderWalletId int,
@senderNewWalletBalance decimal(38,20),
@receiverWalletId int,
@receiverNewWalletBalance decimal(38,20)

AS
BEGIN

insert into Events (UserId, Type, Value, Comment, WhenDate, CurrencyAcronim)
values (@senderUserId, @typeSend, @value, @senderCommentEvent, GETDATE(), @currencyAcronim)

insert into Events (UserId, Type, Value, Comment, WhenDate, CurrencyAcronim)
values (@receiverUserId, @typeRecieve, @value, @receiverCommentEvent, GETDATE(), @currencyAcronim)

UPDATE Wallets
SET Value = @senderNewWalletBalance, LastUpdate = GETDATE()
WHERE ID = @senderWalletId

UPDATE Wallets
SET Value = @receiverNewWalletBalance, LastUpdate = GETDATE()
WHERE Id = @receiverWalletId

END

