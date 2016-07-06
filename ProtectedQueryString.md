# Protecting a querystring from being tampered with

Sometimes changing the querystring of a URL could expose private information. For example, if you change the id on a form it might expose the data from another user. To protect against this you can hash the querystring with a secret salt, and include that hash in the querystring. Checking that the hash still matches when the page is loaded ensures the querystring has not been tampered with. The `UrlProtector` class enables this protection.

Before you redirect to a URL that needs to be protected, call `ProtectQueryString` on the url. 

You need to supply a salt, which should be unique to the resource being protected and is typically stored with that resource. The second constructor parameter is optional and can be any key valid in a URL, so long as it is used consistently in your application.

	var redirectToUrl = new Uri("https://hostname/protectme?id=1");
	var uniqueSalt = Guid.NewGuid().ToString();
	var protector = new UrlProtector(uniqueSalt, "hash");
	var protectedUrl = protector.ProtectQueryString(redirectToUrl);
	Http.Status303SeeOther(protectedUrl);

On the next page, before you trust the `id` parameter in the querystring, check that is has not been tampered with. You'll need to supply the same salt you used to protect the URL:

	var protector = new UrlProtector(rememberedUniqueSalt, "hash");
	var safeUrl = protector.CheckProtectedQueryString(Request.Url);
	if (safeUrl) 
	{
		// application code here
	}
	else
	{
		Http.Status400BadRequest();
		Response.End();
	}