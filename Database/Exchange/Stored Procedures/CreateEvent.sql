USE [Exchange]
GO
/****** Object:  StoredProcedure [dbo].[CreateEvent]    Script Date: 12/16/2021 10:48:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[CreateEvent]
@userid nvarchar(450),
@type int,
@value decimal(38,20) NULL,
@startBalance decimal (38, 20) NULL,
@resultBalance decimal (38, 20) NULL,
@comment nvarchar(450),
@whenDate datetime,
@currencyAcronim nvarchar(450)
AS
BEGIN

INSERT INTO [Exchange].[dbo].[Events] (UserId, Type, Value, StartBalance, ResultBalance, Comment, WhenDate, CurrencyAcronim)
VALUES (@userid, @type, @value, @startBalance, @resultBalance, @comment, @whenDate, @currencyAcronim)

END

