ALTER PROCEDURE [dbo].[GetLastRates]
AS
BEGIN

select r.* from   [Rates] r
inner join (SELECT [Acronim], [Site], Max([Date]) as dateres
FROM [web-api.online].[dbo].[Rates]
group by Acronim, [Site]) as se
on se.[Acronim] = r.[Acronim] and se.[Site] = r.[Site] and se.dateres = r.Date

END
