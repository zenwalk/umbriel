1. Unzip into a new directory (e.g. "c:\Program Files\Umbriel\Umbriel.ArcMapUI")
2. Open a command window.
3. Type:
	regasm.exe <path to Umbriel.ArcMapUI.dll> /codebase
	
	Example:
	regasm.exe "c:\Program Files\Umbriel\Umbriel.ArcMapUI\Umbriel.ArcMapUI.dll" /codebase
	
	-- or --
	
	c:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe "c:\Program Files\Umbriel\Umbriel.ArcMapUI\Umbriel.ArcMapUI.dll" /codebase
	

The Umbriel ArcMap Toolbar contains 4 commands/tools by default; however, these tools can be modified by removing/adding ProgIDs to the CommandBarItems table in the sqlite database, UmbrielArcMapToolbar.sqlite (must exist in the same directory as Umbriel.ArcMapUI.dll).

The 4 Default Tools are:

1. Google Street View - Opens default web browser to Google Maps Street View at the "roughly" the same extent as the map in ArcMap.

2. GeohashID - Click on the map to display the geohash of the mouse cursor.

3. Select Stack Geometries - Analyzes features for identical geometries. Similar to stack finder except it returns a feature selection of "stacked" features.  You must first highlight the layer in the TOC.

4. Add Photo Point - Creates a featureclass for the photos w/ GPS points in them.


	
