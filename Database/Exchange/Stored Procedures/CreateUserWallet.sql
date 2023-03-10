USE [Exchange]
GO
/****** Object:  StoredProcedure [dbo].[CreateUserWallet]    Script Date: 13.12.2021 19:47:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[CreateUserWallet]
@userid nvarchar(450),
@address nvarchar(max),
@currencyAcronim nvarchar(10),
@value decimal(38,20),
@new_identity    INT    OUTPUT
AS
BEGIN

INSERT INTO [Exchange].[dbo].[Wallets] (UserId, Address, CurrencyAcronim, Value)
VALUES (@userid, @address, @currencyAcronim, @value)

SET @new_identity = SCOPE_IDENTITY()
END
