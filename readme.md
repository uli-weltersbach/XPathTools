This is an extension intended for Visual Studio 2012 and 2013. It's available through _Tools &rarr; Extensions and Updates... &rarr; Online_ from within Visual Studio, by searching for "XPath Information" or simply "xpath".

The extension displays the XPath of the XML-element or -attribute at the caret position when working in files containing XML markup (.config, .build, .properties etc.).  
The information is displayed in the statusbar and can be copied to the clipboard in various formats, using the _Copy XPath_ commands available through the context menu.  

Version 2.1 allows for a "shallow XML structure" to be copied to the clipboard, containing the current element, it's ancestors and descendants; other siblings are excluded. This function is primarily intended for use with [Sitecore CMS](http://www.sitecore.net) [Web.config include files](http://www.sitecore.net/Community/Technical-Blogs/John-West-Sitecore-Blog/Posts/2011/05/All-About-Web-config-Include-Files-with-the-Sitecore-ASPNET-CMS.aspx).

The extension has been tested with Visual Studio 2012 and Visual Studio 2013 but might work with older versions if tweaked.