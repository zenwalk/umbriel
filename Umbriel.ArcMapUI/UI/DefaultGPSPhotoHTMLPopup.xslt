<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
  <!ENTITY nbsp "&#160;">
]>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    
    <html>
      <style type="text/css">
      .FieldName {
      font-family: Verdana, Arial, Helvetica, sans-serif;
      font-weight: bold;
      }

   

      </style>
      <body>
        <h2>
          GPS Photo:

          <xsl:for-each select="FieldsDoc/Fields/Field">
            <xsl:if test="FieldName = 'Filename'">
              <xsl:value-of select="FieldValue"/>
            </xsl:if>
          </xsl:for-each>

        </h2>
        <BR />
        <SPAN class="FieldName">Date Taken:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</SPAN>
        
          <xsl:for-each select="FieldsDoc/Fields/Field">
            <xsl:if test="FieldName = 'DatePhotoTaken'">
              <xsl:value-of select="FieldValue"/>
            </xsl:if>
          </xsl:for-each>
        <BR />
        <BR />
        <table width="300" border="1">
          <tr bgcolor="#9acd32">
            <th>Latitude</th>
            <th>Longitude</th>
          </tr>
          <tr>
            <th>
              <xsl:for-each select="FieldsDoc/Fields/Field">
                <xsl:if test="FieldName = 'Latitude'">
                  <xsl:value-of select="FieldValue"/>
                </xsl:if>
              </xsl:for-each>
            </th>
            <th>
              <xsl:for-each select="FieldsDoc/Fields/Field">
                <xsl:if test="FieldName = 'Longitude'">
                  <xsl:value-of select="FieldValue"/>
                </xsl:if>
              </xsl:for-each>
            </th>
          </tr>
        </table>

        <BR></BR>

        <xsl:for-each select="FieldsDoc/Fields/Field">
          <xsl:if test="FieldName = 'FullPath'">
            <xsl:variable  name="imgsrcpath" select="FieldValue"/>
            <a href="{$imgsrcpath}" target="_blank">
              <img width="500"   src="{$imgsrcpath}" />
            </a>
          </xsl:if>
        </xsl:for-each>

        <hr width="600"></hr>
      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>
