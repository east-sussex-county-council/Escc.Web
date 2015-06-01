# Protecting a querystring from being tampered with

Sometimes changing the querystring of a URL could expose private information. For example, if you change the id on a form it might expose the data from another user. To protect against this you can hash the querystring with a secret salt, and include that hash in the querystring. Checking that the hash still matches when the page is loaded ensures the querystring has not been tampered with.

The `Iri` class contains two methods to enable this protection.

	public static Uri ProtectQueryString(Uri urlToProtect, string hashParameter)
    public static bool CheckProtectedQueryString(Uri protectedUrl, string hashParameter)

Before you redirect to a URL that needs to be protected, call `ProtectQueryString` on the url. The second parameter can be any key valid in a URL, so long as it is used consistently in your application.

	var redirectToUrl = new Uri("https://hostname/protectme?id=1");
	var protectedUrl = Iri.ProtectQueryString(redirectToUrl, "h");
	Http.Status303SeeOther(protectedUrl);

On the next page, before you trust the `id` parameter in the querystring, check that is has not been tampered with:

	var safeUrl = Iri.CheckProtectedQueryString(Request.Url, "h");
	if (safeUrl) 
	{
		// application code here
	}
	else
	{
		Http.Status400BadRequest();
		Response.End();
	}

This code requires an application-wide salt to be present in `web.config`. A future version of this code should update this to require a separate salt for each protected resource (ie the data identified by an id).

	<configuration>
	  <configSections>
	    <sectionGroup name="EsccWebTeam.Data.Web">
	      <section name="ProtectedQueryString" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
	    </sectionGroup>
	  </configSections>
	
	  <!-- Enter a random string to use as the salt for hashing the query string-->
	  <EsccWebTeam.Data.Web>
	    <ProtectedQueryString>
	      <add key="Salt" value="" />
	    </ProtectedQueryString>
	  </EsccWebTeam.Data.Web>
	</configuration>