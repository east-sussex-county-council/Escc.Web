# Abbreviate URLs into a version more suitable for display

When presenting URLs to users, in search results or a table for example, they can often be too long and create problems with text wrapping. The `UrlPresenter` class provides a way to abbreviate URLs for display by selecting the most relevant parts.

	// For example, a page returned as a search result
	var longUrl = new Uri("https://www.example.org/make-this-shorter-please/its-far-too-big/as-an-example?including=thisbit");

	// For example, the page displaying the search results
	var currentUrl = new Uri("https://example.org");

	string abbreviatedUrl = new UrlPresenter().AbbreviateUrl(longUrl, currentUrl);