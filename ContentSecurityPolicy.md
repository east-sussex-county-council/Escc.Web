# Content Security Policy

A [Content Security Policy](http://www.html5rocks.com/en/tutorials/security/content-security-policy/) restricts which resources a web page can load, protecting against risks such as cross-site scripting.

## Apply a default policy
Apply a default policy across a website using an HTTP module:

**web.config**

	<configSections>
	  <sectionGroup name="Escc.Web">
	    <section name="ContentSecurityPolicies" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
	  </sectionGroup>
	</configSections>
	
	<Escc.Web>
	  <ContentSecurityPolicies>
	    <add key="Default" value="default-src 'none'; script-src 'self'; style-src 'self'; img-src 'self'" />
	  </ContentSecurityPolicies>
	</Escc.Web>
	
	<system.webServer>
	  <modules>
	    <add name="ContentSecurityPolicy" type="Escc.Web.ContentSecurityPolicyModule"/>
	  </modules>
	</system.webServer>

You can exclude URLs from the default policy using a semi-colon-separated list in a settings section:

**web.config**

	<configSections>
	  <sectionGroup name="Escc.Web">
	    <section name="ContentSecurityPolicySettings" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
	  </sectionGroup>
	</configSections>
	
	<Escc.Web>
	  <ContentSecurityPolicySettings>
		<add key="UrlsToExclude" value="/not-this-folder;/not-this-file.aspx" />
	  </ContentSecurityPolicySettings>
	</Escc.Web>

As well as the default policy, another policy called `Local` is always loaded if present. This allows a development setup to add extra permissions which do not have to be replicated in a live environment, or an application to add its own local settings when the `Default` policy is used sitewide.

## Apply a custom policy using web.config

You can specify a semi-colon-separated list of policy names to apply by default. The default value of `PoliciesToApply` is `Default;Local` as described above. You can customise this to append a policy with extra privileges:

**web.config**

	  <Escc.Web>
	    <ContentSecurityPolicies>
	      <add key="Default" value="default-src 'none'; script-src 'self'; style-src 'self'; img-src 'self'" />
		  <add key="GoogleApi" value="script-src https://apis.google.com" />
	    </ContentSecurityPolicies>
		<ContentSecurityPolicySettings>
		  <add key="PoliciesToApply" value="Default;GoogleApi" />
		</ContentSecurityPolicySettings>
	  </Escc.Web>

The 'GoogleApi' policy is added to the 'Default' policy, creating the following content security policy:

	default-src 'none'; script-src 'self' https://apis.google.com; style-src 'self'; img-src 'self'


## Apply a custom policy using code
The default policy can be customised for a specific page by appending a policy with extra privileges:

**web.config**

	  <Escc.Web>
	    <ContentSecurityPolicies>
	      <add key="Default" value="default-src 'none'; script-src 'self'; style-src 'self'; img-src 'self'" />
		  <add key="GoogleApi" value="script-src https://apis.google.com" />
	    </ContentSecurityPolicies>
	  </Escc.Web>

**C#**

    var policies = new ContentSecurityPolicyFromConfig().Policies;
    
	new ContentSecurityPolicyHeaders(HttpContext.Current.Response.Headers)
	    .AppendPolicy(policies["GoogleApi"]);
	    .UpdateHeaders();

The 'GoogleApi' policy is added to the 'Default' policy, creating the following content security policy:

	default-src 'none'; script-src 'self' https://apis.google.com; style-src 'self'; img-src 'self'

### Applying a custom policy in code without the HTTP module

You can also apply a content security policy to a single page without using the HTTP module or putting anything in `web.config`:

	new ContentSecurityPolicyHeaders(HttpContext.Current.Response.Headers)
	    .AppendPolicy("default-src 'none'; script-src 'self'; style-src 'self'; img-src 'self'");
	    .UpdateHeaders();


