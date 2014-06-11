Content Security Policy
=======================

A [Content Security Policy](http://www.html5rocks.com/en/tutorials/security/content-security-policy/) restricts which resources a web page can load, protecting against risks such as cross-site scripting.

Apply a default policy across a website using an HTTP module:

**web.config**

	  <configSections>
	    <sectionGroup name="Escc.Data.Web">
	      <section name="ContentSecurityPolicy" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
	    </sectionGroup>
	  </configSections>
	
	  <Escc.Data.Web>
	    <ContentSecurityPolicy>
	      <add key="Default" value="default-src 'none'; script-src 'self'; style-src 'self'; img-src 'self'" />
	    </ContentSecurityPolicy>
	  </Escc.Data.Web>
	
	  <system.webServer>
	    <modules>
	      <add name="ContentSecurityPolicy" type="EsccWebTeam.Data.Web.ContentSecurityPolicyModule"/>
	    </modules>
	  </system.webServer>

This can be customised for a specific page by appending a policy with extra privileges:

**web.config**

	  <Escc.Data.Web>
	    <ContentSecurityPolicy>
	      <add key="Default" value="default-src 'none'; script-src 'self'; style-src 'self'; img-src 'self'" />
		  <add key="GoogleApi" value="script-src https://apis.google.com" />
	    </ContentSecurityPolicy>
	  </Escc.Data.Web>

**C#**

    var policy = new ContentSecurityPolicy(HttpContext.Current.Request.Url);
    policy.AppendFromConfig("GoogleApi");
    policy.UpdateHeader(System.Web.HttpContext.Current.Response);

The 'GoogleApi' policy is added to the 'Default' policy, creating the following content security policy:

	default-src 'none'; script-src 'self' https://apis.google.com; style-src 'self'; img-src 'self'

You can also exclude URLs from the default policy using a semi-colon-separated list for a policy of 'None':

**web.config**

	<Escc.Data.Web>
	  <ContentSecurityPolicy>
	    <add key="Default" value="default-src 'none'; script-src 'self'; style-src 'self'; img-src 'self'" />
		<add key="None" value="/not-this-folder;/not-this-file.aspx" />
	  </ContentSecurityPolicy>
	</Escc.Data.Web>