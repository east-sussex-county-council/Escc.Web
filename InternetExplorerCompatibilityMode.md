Internet Explorer Compatibility Mode
====================================

We don't want to use Internet Explorer's compatibility mode, because we want to use the latest version of Internet Explorer available. Sometimes we need it though, where old applications rely on bugs in old versions of Internet Explorer. 

The `InternetExplorerCompatibilityModeModule` allows compatibility mode to be applied selectively, so that we're not forced to run an entire site in compatibility mode. It is configured using the following settings in `web.config`.

	<configuration>

	  <configSections>
	    <sectionGroup name="EsccWebTeam.Data.Web">
	      <section name="InternetExplorerCompatibilityMode" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
	    </sectionGroup>
	  </configSections>

	  <EsccWebTeam.Data.Web>
	    <InternetExplorerCompatibilityMode>
	      <add key="/path/to/affected/page.aspx" value="IE=8" />
	      <add key=".*" value="IE=Edge" />
	    </InternetExplorerCompatibilityMode>
	  </EsccWebTeam.Data.Web>

	  <system.web>
	    <httpModules>
	      <add name="InternetExplorerCompatibilityMode" type="EsccWebTeam.Data.Web.InternetExplorerCompatibilityModeModule, EsccWebTeam.Data.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=06fad7304560ae6f" />
	    </httpModules>
	  </system.web>

	</configuration>

The `<InternetExplorerCompatibilityMode>` section can contain as many rules as necessary for the site. 

The `key` attribute specifies a regular expression to match the requested URL. The second rule shown in the example uses a regular expression of `.*`, which matches any URL and is therefore a way of specifying a sitewide rule.

The `value` attribute can be any valid value for the `X-UA-Compatible` HTTP header. The [allowed values](http://msdn.microsoft.com/en-us/library/ff955275(v=vs.85).aspx) are listed on MSDN.  