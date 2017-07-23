using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using System.IO;

namespace ReasonCodeExample.XPathTools.Tests
{
    [TestFixture]
    public class XmlRepositoryTests
    {
        [TestCase("<a />", 1, "")]
        [TestCase("<a />", 2, "a")]
        [TestCase("<node />", 2, "node")]
        [TestCase("<ns:node xmlns:ns=\"ns-namespace\"/>", 2, "{ns-namespace}node")]
        [TestCase("<parent><child /></parent>", 2, "parent")]
        [TestCase("<parent><child /></parent>", 10, "child")]
        [TestCase("<parent><child /></parent>", 17, "child")]
        [TestCase("<parent><child1 /><child2 /><child3></child3></parent>", 10, "child1")]
        [TestCase("<parent><sibling></sibling><node /></parent>", 29, "node")]
        [TestCase("<parent><child><grandChild><grandGrandChild /></grandChild><node /></child></parent>", 61, "node")]
        [TestCase("<node1><node2><node3><node4 /></node3><node5 /></node2></node1>", 16, "node3")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><o:node /></grandChild></child></parent>", 67, "{o-namespace}node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><o:node><node /></o:node></grandChild></child></parent>", 75, "{default}node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><node><node xmlns:x=\"x-namespace\"><x:node /></node></node></grandChild></child></parent>", 101, "{x-namespace}node")]
        public void ElementIsFound(string xml, int linePosition, string expectedElementName)
        {
            // Arrange
            var repository = new XmlRepository();
            var rootElement = XElement.Parse(xml, LoadOptions.SetLineInfo);

            // Act
            var element = repository.GetElement(rootElement, 1, linePosition);
            var elementName = element == null ? string.Empty : element.Name.ToString();

            // Assert
            Assert.That(elementName, Is.EqualTo(expectedElementName));
        }

        [TestCase("<node name=\"value\" otherName=\"value\"/>", 7, "name")]
        [TestCase("<node name=\"value\" otherName=\"value\"/>", 18, "name")]
        [TestCase("<node name=\"value\" otherName=\"value\"/>", 20, "otherName")]
        [TestCase("<ns:node xmlns:ns=\"ns-namespace\" ns:attribute=\"value\"/>", 10, "{http://www.w3.org/2000/xmlns/}ns")]
        [TestCase("<ns:node xmlns:ns=\"ns-namespace\" ns:attribute=\"value\"/>", 34, "{ns-namespace}attribute")]
        public void AttributeIsFound(string xml, int linePosition, string expectedAttributeName)
        {
            // Arrange
            var repository = new XmlRepository();
            var rootElement = XElement.Parse(xml, LoadOptions.SetLineInfo);
            const int lineNumber = 1;

            // Act
            var attribute = repository.GetAttribute(rootElement, lineNumber, linePosition);
            var attributeName = attribute == null ? string.Empty : attribute.Name.ToString();

            // Assert
            Assert.That(attributeName, Is.EqualTo(expectedAttributeName));
        }

        [TestCase(13, 41, "site")]
        [TestCase(14, 41, "site")]
        [TestCase(20, 41, "site")]
        [TestCase(22, 41, "")]
        public void MultiLineElementIsFound(int lineNumber, int linePosition, string expectedElementName)
        {
            // Arrange
            var xml = @"<configuration xmlns:patch=""http://www.sitecore.net/xmlconfig/"">
                              <sitecore>
                                <sites>
                                  <site name=""website"">
                                    <patch:attribute name=""rootPath"">/sitecore/content/Original</patch:attribute>
                                  </site>
      
                                  <site name=""OtherWebsite"" patch:after=""site[@name='website']""
                                        virtualFolder=""/""
                                        physicalFolder=""/""
                                        rootPath=""/sitecore/content/OtherWebsite""
                                        startItem=""/Home""
                                        database=""web""
                                        domain=""extranet""
                                        allowDebug=""true""
                                        cacheHtml=""true""
                                        htmlCacheSize=""10MB""
                                        enablePreview=""true""
                                        enableWebEdit=""true""
                                        enableDebugger=""true""
                                        disableClientData=""false""/>
                                </sites>
                              </sitecore>
                            </configuration>";
            var rootElement = XElement.Parse(xml, LoadOptions.SetLineInfo);
            var siteElement = rootElement.DescendantsAndSelf("site").LastOrDefault();
            var repository = new XmlRepository();

            // Act
            var element = repository.GetElement(siteElement, lineNumber, linePosition);
            var elementName = element == null ? string.Empty : element.Name.ToString();

            // Assert
            Assert.That(elementName, Is.EqualTo(expectedElementName));
        }

        [TestCase(13, 41, "database")]
        [TestCase(14, 41, "domain")]
        [TestCase(20, 41, "enableDebugger")]
        [TestCase(22, 41, "")]
        public void MultiLineElementAttributeIsFound(int lineNumber, int linePosition, string expectedAttributeName)
        {
            // Arrange
            var xml = @"<configuration xmlns:patch=""http://www.sitecore.net/xmlconfig/"">
                              <sitecore>
                                <sites>
                                  <site name=""website"">
                                    <patch:attribute name=""rootPath"">/sitecore/content/Original</patch:attribute>
                                  </site>
      
                                  <site name=""OtherWebsite"" patch:after=""site[@name='website']""
                                        virtualFolder=""/""
                                        physicalFolder=""/""
                                        rootPath=""/sitecore/content/OtherWebsite""
                                        startItem=""/Home""
                                        database=""web""
                                        domain=""extranet""
                                        allowDebug=""true""
                                        cacheHtml=""true""
                                        htmlCacheSize=""10MB""
                                        enablePreview=""true""
                                        enableWebEdit=""true""
                                        enableDebugger=""true""
                                        disableClientData=""false""/>
                                </sites>
                              </sitecore>
                            </configuration>";
            var rootElement = XElement.Parse(xml, LoadOptions.SetLineInfo);
            var siteElement = rootElement.DescendantsAndSelf("site").LastOrDefault();
            var repository = new XmlRepository();

            // Act
            var attribute = repository.GetAttribute(siteElement, lineNumber, linePosition);
            var attributeName = attribute == null ? string.Empty : attribute.Name.ToString();

            // Assert
            Assert.That(attributeName, Is.EqualTo(expectedAttributeName));
        }

        [TestCase("<a />", "/a", 1)]
        [TestCase("<a><b /><b /><b /><b /><b /></a>", "/a/b", 5)]
        [TestCase("<a><b c=''/><b c=''/><b c=''/><b /><b /></a>", "/a/b/@c", 3)]
        public void ElementCountIsCorrect(string xml, string xpath, int expectedCount)
        {
            // Arrange
            var document = XDocument.Parse(xml);
            var repository = new XmlRepository();

            // Act
            var count = repository.GetNodeCount(document.Root, xpath);

            // Assert
            Assert.That(count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void ResetXmlOnLoad()
        {
            // Arrange
            var repository = new XmlRepository();
            repository.LoadXml("<this-is-xml />", null);

            // Act
            repository.LoadXml("This is not XML.", null);

            // Assert
            Assert.That(repository.GetRootElement(), Is.Null);
        }

        [Test]
        public void HandlesAbsoluteeDtdReferencesGracefully()
        {
            // Arrange
            var xmlFilePath = Path.GetTempFileName();
            File.WriteAllText(xmlFilePath, XML);

            var dtdFilePath = Path.GetTempFileName();
            File.WriteAllText(dtdFilePath, DTD);

            var xml = File.ReadAllText(xmlFilePath).Replace(DtdFilePlaceholder, dtdFilePath);
            var repository = new XmlRepository();

            // Act
            repository.LoadXml(xml, null);

            // Assert
            Assert.That(repository.GetRootElement(), Is.Not.Null);
        }

        [Test]
        public void HandlesRelativeDtdReferencesGracefully()
        {
            // Arrange
            var xmlFilePath = Path.GetTempFileName();
            File.WriteAllText(xmlFilePath, XML);

            var dtdFilePath = Path.GetTempFileName();
            File.WriteAllText(dtdFilePath, DTD);
            var dtdFileName = Path.GetFileName(dtdFilePath);

            var xml = File.ReadAllText(xmlFilePath).Replace(DtdFilePlaceholder, dtdFileName);
            var repository = new XmlRepository();

            // Act
            repository.LoadXml(xml, xmlFilePath);

            // Assert
            Assert.That(repository.GetRootElement(), Is.Not.Null);
        }

        #region XML
        private const string DtdFilePlaceholder = "<DTD FILE RELATIVE OR ABSOLUTE URI>";
        private const string XML = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE database SYSTEM """ + DtdFilePlaceholder + @""">
<database>
  <server id=""v12"" lang=""cz"">
    <continent name=""Kalimdor"">
      <player id=""v12354"">
        <name>Bimbinbiribong</name>
        <class>&lock;</class>
        <level>75</level>
        <health>123</health>
        <mana>155</mana>
        <attributes>
          <strength>12</strength>
          <agility>10</agility>
          <stamina>15</stamina>
          <intellect>16</intellect>
          <spirit>20</spirit>
          <armor>67</armor>
        </attributes>
        <buffs>
          <buff id=""u1532"">
            <name>Power Word: Fortitude</name>
            <duration timeUnit=""minute"">60</duration>
            <description>
              Gives<highlight>2 stamina</highlight> to owner.
            </description>
          </buff>
        </buffs>
        <debuffs/>
        <location>
          <area>Elwynn Forest</area>
          <city>Stormwind City</city>
        </location>
        <icon src=""blablablablabla.jpg""/>
      </player>
      <player id=""v154"">
        <name>Charles Xavier</name>
        <class>hunter</class>
        <level>70</level>
        <health>123</health>
        <mana>155</mana>
        <attributes>
          <strength>12</strength>
          <agility>10</agility>
          <stamina>15</stamina>
          <intellect>16</intellect>
          <spirit>20</spirit>
          <armor>67</armor>
        </attributes>
        <buffs>
          <buff id=""u153"">
            <name>Divine Shield</name>
            <duration timeUnit=""second"">12</duration>
            <description>Makes you invincible for given duration.</description>
          </buff>
          <buff id=""a123"">
            <name>Seal of Righteousness</name>
            <duration timeUnit=""second"">20</duration>
            <description>Makes you hit by 124 dmg more for every hit.</description>
          </buff>
        </buffs>
        <debuffs>
          <debuff id=""f222"">
            <name>Corruption</name>
            <duration timeUnit=""second"">22</duration>
            <description>Deals 222 dmg every 3 seconds.</description>
          </debuff>
        </debuffs>
        <location>
          <area>Elwynn Forest</area>
          <city>Stormwind City</city>
        </location>
        <icon src=""blablablablabla.jpg""/>
      </player>
      <pet id=""a95"" ownerID=""v12354"">
        <name>Lakatos</name>
        <type>Imp</type>
      </pet>
      <pet id=""u95"" ownerID=""v12354"">
        <name>Laco</name>
        <type>Imp</type>
      </pet>
      <player id=""v1549"">
        <name>KingKong</name>
        <class>hunter</class>
        <level>70</level>
        <health>111</health>
        <mana>222</mana>
        <attributes>
          <strength>8</strength>
          <agility>9</agility>
          <stamina>18</stamina>
          <intellect>19</intellect>
          <spirit>10</spirit>
          <armor>100</armor>
        </attributes>
        <buffs/>
        <debuffs/>
        <location>
          <area>Elwynn Forest</area>
          <city>Stormwind City</city>
        </location>
        <icon src=""blablablablabla.jpg""/>
      </player>
    </continent>
  </server>
  <server id=""v13"" lang=""cz"">
    <continent name=""Azeroth"">
      <player id=""v12359"">
        <name>Uff</name>
        <class>warlock</class>
        <level>75</level>
        <health>123</health>
        <mana>155</mana>
        <attributes>
          <strength>9</strength>
          <agility>10</agility>
          <stamina>15</stamina>
          <intellect>16</intellect>
          <spirit>20</spirit>
          <armor>67</armor>
        </attributes>
        <buffs/>
        <debuffs>
          <debuff id=""u15"">
            <name>Shadow Word: Pain</name>
            <duration timeUnit=""second"">21</duration>
            <description>
              Gives<highlight>2 damage</highlight> every 3 seconds.
            </description>
          </debuff>
        </debuffs>
        <location>
          <area>Elwynn Forest</area>
          <city>Stormwind City</city>
        </location>
        <icon src=""blablablablabla.jpg""/>
      </player>
      <pet id=""a915"" ownerID=""v12354"">
        <name>Lakatos</name>
        <type>Imp</type>
      </pet>
      <pet id=""b915"" ownerID=""v12359"">
        <name>Lakatos</name>
        <type>Imp</type>
      </pet>
      <pet id=""c915"" ownerID=""v12359"">
        <name>Lakatos</name>
        <type>Imp</type>
      </pet>
    </continent>
  </server>
  <appendix>
    <![CDATA[
document.writeln(""All rights to me and Blizzard"");
  ]]>
  </appendix>
  <!-- date when it was generated  -->
  <date>
    <?php echo ""Generated at"" . date('l jS \of F Y h:i:s A') ?>
  </date>
</database>";
        #endregion

        #region DTD
        private const string DTD = @"<!ELEMENT database (server*, appendix?, date?)>
<!ELEMENT appendix ANY>
<!ELEMENT server (continent+ )>
<!ELEMENT date (#PCDATA)>
<!ATTLIST server
id CDATA #REQUIRED
lang (en | cz | de | sp) ""en"">
<!ELEMENT continent (player | gameObject | pet)*>
<!ATTLIST continent name (Azeroth | Outland) #REQUIRED>
<!ELEMENT player (name , class , level , health , mana , attributes , buffs , debuffs , location, icon)>
<!ATTLIST player id ID #REQUIRED>
<!ELEMENT health (#PCDATA)>
<!ELEMENT mana (#PCDATA)>
<!ELEMENT name (#PCDATA)>
<!ELEMENT class (#PCDATA)>
<!ELEMENT level (#PCDATA)>
<!ELEMENT buffs ( buff)*>
<!ELEMENT debuffs (debuff)*>
<!ELEMENT buff (name , duration , description)>
<!ATTLIST buff id ID #REQUIRED>
<!ELEMENT duration (#PCDATA)>
<!ATTLIST duration timeUnit (second | minute | hour) ""second"">
<!ELEMENT description (#PCDATA | highlight)*>
<!ELEMENT highlight (#PCDATA)>
<!ELEMENT debuff (name , duration , description)>
<!ATTLIST debuff id ID #REQUIRED>
<!ELEMENT attributes (strength , agility , stamina , intellect , spirit , armor)>
<!ELEMENT strength (#PCDATA)>
<!ELEMENT stamina (#PCDATA)>
<!ELEMENT agility (#PCDATA)>
<!ELEMENT intellect (#PCDATA)>
<!ELEMENT spirit (#PCDATA)>
<!ELEMENT armor (#PCDATA)>
<!ELEMENT gameObject (type)>
<!ATTLIST gameObject id ID #REQUIRED>
<!ELEMENT type (#PCDATA)>
<!ELEMENT location (area , city?)>
<!ELEMENT area (#PCDATA)>
<!ELEMENT city (#PCDATA)>
<!ELEMENT pet (name , type)>
<!ATTLIST pet 
id ID #REQUIRED
ownerID IDREF #REQUIRED>
<!ENTITY lock ""warlock"">
<!ENTITY war ""warrior"">
<!ELEMENT icon EMPTY>
<!ATTLIST icon src CDATA #REQUIRED>";
        #endregion
    }
}