
--Enable CLR and reconfigure (uncomment if needed)
/*
sp_Configure 'CLR Enabled', 1

RECONFIGURE
go
*/


create assembly GeohashFunctions From 'C:\svn_workspace\umbriel.googlecode.com\trunk\Umbriel.GIS.ClrFunctions\bin\Release\Umbriel.GIS.GeohashFunctions.dll'


--DROP FUNCTION dbo.EncodeCoordinate
--DROP FUNCTION GeohashLongitude
--DROP FUNCTION GeohashLatitude
--DROP ASSEMBLY ClrFunctions

--Function that calls the [Umbriel.GIS.GeohashFunctions.Functions] EncodeCoordinate method.
Create Function dbo.EncodeCoordinate(
@latitude As float,
@longitude As float
)
Returns nvarchar(25)
As External Name [GeohashFunctions].[Umbriel.GIS.GeohashFunctions.Functions].EncodeCoordinate;
GO

--Function that calls the [Umbriel.GIS.GeohashFunctions.Functions] GeohashLongitude method.
Create Function dbo.GeohashLongitude(
	@geohash as nvarchar(25)
)
Returns float
As External Name [GeohashFunctions].[Umbriel.GIS.GeohashFunctions.Functions].GeohashLongitude;
GO

--Function that calls the [Umbriel.GIS.GeohashFunctions.Functions] GeohashLatitude method.
Create Function dbo.GeohashLatitude(
	@geohash as nvarchar(25)
)
Returns float
As External Name [GeohashFunctions].[Umbriel.GIS.GeohashFunctions.Functions].GeohashLatit