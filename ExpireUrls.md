# Giving URLs an expiry date

Sometimes you want a URL only to work for a limited period of time, perhaps for privacy reasons so that it cannot easily be shared. You can do this by putting a date in the URL, which you check against your deadline in your application code, and then using a hash to verify that the URL has not been tampered with. The `UrlExpirer` class enables this protection.

These automatically [protect the querystring from being tampered with](ProtectedQueryString.md), so you will need to add the configuration required by that feature.

Before you redirect to a URL that needs to have an expiry date, call `ExpireUrl` on the url. 

	var redirectToUrl = new Uri("https://hostname/protectme?id=1");
	var uniqueSalt = Guid.NewGuid().ToString();
	var expirer = new UrlExpirer(new UrlProtector(uniqueSalt, "hash"), "time");
	var protectedUrl = expirer.ExpireUrl(redirectToUrl);
	Http.Status303SeeOther(protectedUrl);

The `hash` and `time` parameters shown here are optional overrides for default values, and can be any key valid in a URL, so long as it is used consistently in your application.

On the next page, before you trust the `id` parameter in the querystring, check that the URL has not expired or been tampered with. This shows a time limit of 1 hour:

	var timeLimitInSeconds = 3600;
	var expirer = new UrlExpirer(new UrlProtector(rememberedUniqueSalt, "hash"), "time");
	var expired = expirer.HasUrlExpired(Request.Url, timeLimitInSeconds);
	if (!expired) 
	{
		// application code here
	}
	else
	{
		Http.Status400BadRequest();
		Response.End();
	}