SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetP2PCrypts]
AS
BEGIN

SELECT * FROM [Exchange].[dbo].[P2PCrypts] 

END
