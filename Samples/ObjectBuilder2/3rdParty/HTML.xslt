<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html"/>
  <xsl:template match="/">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="assembly">
    <xsl:text disable-output-escaping="yes"><![CDATA[<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">]]></xsl:text>
    <html>
      <head>
        <title>xUnit.net Test Results - <xsl:value-of select="@name"/>
        </title>
        <style type="text/css">
          body { font-family: Calibri, Verdana, Arial, sans-serif; background-color: White; color: Black; }
          h2,h3,h5 { margin: 0; padding: 0; }
          h2 { border-top: solid 1px #f0f5fa; padding-top: 0.5em; }
          h3 { font-weight: normal; }
          h5 { font-weight: normal; font-style: italic; margin-bottom: 0.75em; }
          pre { font-family: Consolas; font-size: 85%; margin: 0 0 0 1em; padding: 0; }
          .row, .altrow { padding: 0.1em 0.3em; }
          .row { background-color: #f8fafc; }
          .altrow { background-color: #f0f5fa; }
          .success, .failure, .skipped { font-family: Arial Unicode MS; font-weight: normal; float: left; width: 1em; display: block; }
          .success { color: #0c0; }
          .failure { color: #c00; }
          .skipped { color: #cc0; }
          .timing { float: right; }
          .indent { margin: 0.25em 0 0.5em 2em; }
          .clickable { cursor: pointer; }
        </style>
        <script language="javascript">
          function ToggleClass(id) {
            var elem = document.getElementById(id);
            if (elem.style.display == "none")
              elem.style.display = "block";
            else
              elem.style.display = "none";
          }
        </script>
      </head>
      <body>
        <h3><b>Results for <xsl:value-of select="@name"/></b></h3>
        <div>
          Tests run: <a href="#all"><b><xsl:value-of select="@total"/></b></a> &#160;
          Failures: <a href="#failures"><b><xsl:value-of select="@failures"/></b></a>,
          Skipped: <a href="#skipped"><b><xsl:value-of select="@not-run"/></b></a>,
          Run time: <b><xsl:value-of select="@test-time"/>s</b>
        </div>
        <xsl:if test="@failures > 0">
          <br />
          <h2><a name="failures"></a>Failed tests</h2>
          <xsl:apply-templates select="//test[@success='False']"><xsl:sort select="@name"/></xsl:apply-templates>
          <xsl:apply-templates select="//class/failure"><xsl:sort select="../@name"/></xsl:apply-templates>
        </xsl:if>
        <xsl:if test="//test[@executed='False']">
          <br />
          <h2><a name="skipped"></a>Skipped tests</h2>
          <xsl:apply-templates select="//test[@executed='False']"><xsl:sort select="@name"/></xsl:apply-templates>
        </xsl:if>
        <br />
        <h2><a name="all"></a>All tests</h2>
        <h5>Click test class name to expand/collapse test details</h5>
        <xsl:apply-templates select="//class"><xsl:sort select="@name"/></xsl:apply-templates>
        <br /><h5>Results generated <xsl:value-of select="@date"/> at <xsl:value-of select="@time"/></h5>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="test">
    <div>
      <xsl:attribute name="class"><xsl:if test="(position() mod 2 = 0)">alt</xsl:if>row</xsl:attribute>
      <xsl:if test="@executed='True'"><span class="timing"><xsl:value-of select="@time"/>s</span></xsl:if>
      <xsl:if test="@executed='False'"><span class="timing">Skipped</span><span class="skipped">&#x2762;</span></xsl:if>
      <xsl:if test="@success='False'"><span class="failure">&#x2718;</span></xsl:if>
      <xsl:if test="@success='True'"><span class="success">&#x2714;</span></xsl:if>
      &#160;<xsl:value-of select="@name"/>
      <xsl:if test="child::node()/message"> : <xsl:value-of select="child::node()/message"/></xsl:if>
      <br clear="all" />
      <xsl:if test="failure/stack-trace">
        <pre><xsl:value-of select="failure/stack-trace"/></pre>
      </xsl:if>
    </div>
  </xsl:template>

  <xsl:template match="failure">
    <span class="failure">&#x2718;</span> <xsl:value-of select="../@name"/> : <xsl:value-of select="message"/><br clear="all"/>
    Stack Trace:<br />
    <pre><xsl:value-of select="stack-trace"/></pre>
  </xsl:template>

  <xsl:template match="class">
    <h3>
      <span class="timing"><xsl:value-of select="@time"/>s</span>
      <span class="clickable">
        <xsl:attribute name="onclick">ToggleClass('class<xsl:value-of select="position()"/>')</xsl:attribute>
        <xsl:attribute name="ondblclick">ToggleClass('class<xsl:value-of select="position()"/>')</xsl:attribute>
        <xsl:if test="@success='False'"><span class="failure">&#x2718;</span></xsl:if>
        <xsl:if test="@success='True'"><span class="success">&#x2714;</span></xsl:if>
        &#160;<xsl:value-of select="@name"/>
      </span>
      <br clear="all" />
    </h3>
    <div class="indent">
      <xsl:if test="@success='True'"><xsl:attribute name="style">display: none;</xsl:attribute></xsl:if>
      <xsl:attribute name="id">class<xsl:value-of select="position()"/></xsl:attribute>
      <xsl:apply-templates select="test"><xsl:sort select="@name"/></xsl:apply-templates>
    </div>
  </xsl:template>

</xsl:stylesheet>