USE [web-api.online]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[GetUserIdBy_UserName_NormalizedUserName_Email_PhoneNumber]
@searchText nvarchar(max)
AS
BEGIN

Select ANU.Id
FROM AspNetUsers as ANU
WHERE 
ANU.UserName = @searchText
OR ANU.NormalizedUserName = @searchText
OR ANU.Email = @searchText
OR ANU.PhoneNumber = @searchText

END
