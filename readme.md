This is an extension intended for Visual Studio 2012 and 2013. It's available through _Tools &rarr; Extensions and Updates... &rarr; Online_ from within Visual Studio, by searching for "XPath Information" or simply "xpath".

The extension displays the XPath of the XML-element or -attribute at the caret position when working in files containing XML markup (.config, .build, .properties etc.).  
The information is displayed in the statusbar and can be copied to the clipboard in various formats, using the _Copy XPath_ commands available through the context menu.  

The XPath variations currently available are:

* Generic - matches one or more elements
* Absolute - matches the current element by index
* Distinct - attempts to utilize "id", "name" and "type" attributes to create a XPath which only matches the current element 

The "distinct" option has been added mainly for work with [Web.config](http://msdn.microsoft.com/en-us/library/w7w4sb0w.aspx) files.

A "shallow XML structure" can be copied to the clipboard which contains the current element, it's ancestors and descendants; other siblings are excluded. This function is primarily intended for use with [Sitecore CMS](http://www.sitecore.net) [Web.config include files](http://www.sitecore.net/Community/Technical-Blogs/John-West-Sitecore-Blog/Posts/2011/05/All-About-Web-config-Include-Files-with-the-Sitecore-ASPNET-CMS.aspx).

The extension has been tested with Visual Studio 2012 and Visual Studio 2013 but might work with older versions if tweaked; the source is available from https://github.com/uli-weltersbach/XPathInformation.

Please use https://github.com/uli-weltersbach/XPathInformation/issues for feature requests and bug reports.