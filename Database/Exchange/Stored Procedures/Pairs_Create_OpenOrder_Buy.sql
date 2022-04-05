alter PROCEDURE [dbo].[Pairs_Create_OpenOrder_Buy]
@pair nvarchar(20),
@userid nvarchar(450),
@price decimal(38,20),
@amount decimal(38,20),
@total decimal(38,20),
@new_identity bigint OUTPUT
AS
BEGIN

declare @Sql nvarchar(max);

set @Sql = 'INSERT INTO [Exchange].[dbo].[' + @pair + '_OpenOrders_Buy] (Price, Amount, Total, CreateUserId) VALUES (1,1,1,''' + @userid + ''') select SCOPE_IDENTITY()'
exec sp_executesql @sql 

END
