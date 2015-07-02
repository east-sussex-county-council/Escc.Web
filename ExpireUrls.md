# Giving URLs an expiry date

Sometimes you want a URL only to work for a limited period of time, perhaps for privacy reasons so that it cannot easily be shared. You can do this by putting a date in the URL, which you check against your deadline in your application code, and then using a hash to verify that the URL has not been tampered with.

The `Iri` class contains two methods to enable this protection.

	public static Uri ExpireUrl(Uri urlToExpire, string hashParameter, string timeParameter)
    public static bool HasUrlExpired(Uri urlToCheck, string hashParameter, string timeParameter, int validForSeconds)

These automatically [protect the querystring from being tampered with](ProtectedQueryString.md), so you will need to add the configuration required by that feature.

Before you redirect to a URL that needs to have an expiry date, call `ExpireUrl` on the url. The second and third parameters can be any key valid in a URL, so long as it is used consistently in your application.

	var redirectToUrl = new Uri("https://hostname/protectme?id=1");
	var protectedUrl = Iri.ExpireUrl(redirectToUrl, "h", "t");
	Http.Status303SeeOther(protectedUrl);

On the next page, before you trust the `id` parameter in the querystring, check that the URL has not expired or been tampered with. This shows a time limit of 1 hour:

	var timeLimitInSeconds = 3600;
	var expired = Iri.HasUrlExpired(Request.Url, "h", "t", timeLimitInSeconds);
	if (!expired) 
	{
		// application code here
	}
	else
	{
		Http.Status400BadRequest();
		Response.End();
	}