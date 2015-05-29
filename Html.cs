using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace EsccWebTeam.Data.Web
{
    /// <summary>
    /// Methods for working with elements of HTML
    /// </summary>
    [Obsolete("Use the Escc.Html NuGet package")]    
    public static class Html
    {
        /// <summary>
        /// Removes XML/HTML tags from a string, leaving only the text content
        /// </summary>
        /// <param name="text">Text with tags</param>
        /// <returns>Text without tags</returns>
        public static string StripTags(string text)
        {
            return StripTags(text, null);
        }

        /// <summary>
        /// Removes XML/HTML tags from a string, leaving only the text content
        /// </summary>
        /// <param name="text">Text with tags</param>
        /// <param name="allowTagNames">The tag names which are allowed</param>
        /// <returns>Text without tags</returns>
        public static string StripTags(string text, string[] allowTagNames)
        {
            if (allowTagNames != null)
            {
                foreach (string tagName in allowTagNames)
                {
                    text = Regex.Replace(text, "<(/?" + tagName + "[^>]*)>", "{{{$1}}}");
                }
            }

            text = text.Replace("<br />", Environment.NewLine);
            text = text.Replace("</p>", Environment.NewLine + Environment.NewLine);
            text = Regex.Replace(text, "<[^>]*>", "", RegexOptions.IgnoreCase).Trim();

            if (allowTagNames != null)
            {
                foreach (string tagName in allowTagNames)
                {
                    text = Regex.Replace(text, "{{{(/?" + tagName + "[^}]*)}}}", "<$1>");
                }
            }

            return text;
        }

        /// <summary>
        /// Escapes XML/HTML tags in a string so that they appear as literal text within HTML or XML. Use the instance method if you want to allow some tags to remain.
        /// </summary>
        /// <param name="text">The text containing tags to escape.</param>
        /// <returns>Escaped string</returns>
        public static string EscapeTags(string text)
        {
            return EscapeTags(text, null);
        }

        /// <summary>
        /// Escapes XML/HTML tags in a string so that they appear as literal text within HTML or XML.
        /// </summary>
        /// <param name="text">The text containing tags to escape.</param>
        /// <param name="allowTagNames">The names of tags which should be allowed to remain.</param>
        /// <returns>Escaped string</returns>
        public static string EscapeTags(string text, string[] allowTagNames)
        {
            // Work on any individual tag in a structured way
            // Anonymous function allows a match evaluator delegate containing the passed-in variable, where normally the delegate could not receive any custom parameters
            text = Regex.Replace(text, @"(?<OpenTag></?)(?<TagName>[a-z]+)(?<CloseTag> [^>]*>|>)", delegate(Match match) { return EscapeTag_MatchEvaluator(match, allowTagNames); }, RegexOptions.Singleline & RegexOptions.IgnoreCase);

            return text;
        }

        /// <summary>
        /// Parses an opening tag with attributes within the HTML and tidies it up
        /// </summary>
        /// <param name="match">A single matched tag</param>
        /// <param name="allowTagNames">The names of tags which should be allowed to remain.</param>
        /// <returns></returns>
        private static string EscapeTag_MatchEvaluator(Match match, string[] allowTagNames)
        {
            string tag = match.Groups["OpenTag"].Value + match.Groups["TagName"].Value + match.Groups["CloseTag"].Value;
            if (allowTagNames != null && new List<string>(allowTagNames).Contains(match.Groups["TagName"].Value))
            {
                return tag;
            }
            else return HttpUtility.HtmlEncode(tag);
        }

        /// <summary>
        /// Formats a text string, which may already contain some HTML tags, as HTML paragraphs.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string FormatAsHtmlParagraphs(string text)
        {
            if (text == null) return text;

            // establish some strings which will be reused
            string[] blockElements = { "address", "blockquote", "dl", "p", "h1", "h2", "h3", "h4", "h5", "h6", "ol", "table", "ul", "dd", "dt", "li", "tbody", "td", "tfoot", "th", "thead", "tr" };
            string twoNewLines = Environment.NewLine + Environment.NewLine;
            string threeNewLines = Environment.NewLine + Environment.NewLine + Environment.NewLine;
            string lineBreak = "<br />";

            // First, ensure each block element is preceded by at least two newlines, because that helps split them from paragraphs.
            // None of these block elements should end up inside a paragraph.
            text = Regex.Replace(text, "<(" + String.Join("|", blockElements) + ")([^>]*)>", twoNewLines + "<$1$2>");

            // We're going to assume that one newline is a line break and two newlines is a paragraph break, 
            // so ensure there are never more that two consecutive newlines
            int lenBefore = 0;
            int lenAfter = -1;
            while (lenBefore > lenAfter)
            {
                lenBefore = text.Length;
                text = text.Replace(threeNewLines, twoNewLines);
                lenAfter = text.Length;
            }

            // Split the text into chunks based on the separator (two newlines) which should now appear between all block elements
            string[] chunks = text.Split(new string[] { twoNewLines }, StringSplitOptions.RemoveEmptyEntries);

            // Surround each chunk with a paragraph element, if it doesn't already start with a block element
            int lenChunks = chunks.Length;
            for (int i = 0; i < lenChunks; i++)
            {
                bool addParagraphElement = true;
                chunks[i] = chunks[i].Trim();
                foreach (string elementName in blockElements)
                {
                    if (chunks[i].StartsWith("<" + elementName, StringComparison.Ordinal)) addParagraphElement = false;
                }
                if (addParagraphElement) chunks[i] = "<p>" + chunks[i] + "</p>";
            }

            // Join the chunks back together
            text = String.Join(String.Empty, chunks);

            // Any remaining newlines should be line breaks
            text = text.Replace(Environment.NewLine, lineBreak);

            // But remove line breaks which appear just before the close of a block element, because that's definitely a misuse of
            // the line break element and more often than not it creates spacing problems
            foreach (string blockElement in blockElements)
            {
                string closingTag = "</" + blockElement + ">";
                text = text.Replace(lineBreak + closingTag, closingTag);

                // Add a newline after each closing block element, purely to make the HTML source easier to read
                text = text.Replace(closingTag, closingTag + Environment.NewLine);
            }
            return text;
        }

        /// <summary>
        /// Fixes the HTML output from an instance of the Tiny MCE editor
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mce")]
        public static string FixTinyMceOutput(string text)
        {
            string[] blockElements = { "address", "blockquote", "dl", "p", "h1", "h2", "h3", "h4", "h5", "h6", "ol", "table", "ul", "dd", "dt", "li", "tbody", "td", "tfoot", "th", "thead", "tr" };

            // Remove any block elements with no content
            foreach (string elementName in blockElements)
            {
                text = Regex.Replace(text, "<" + elementName + @"[^>]*>\s*</" + elementName + ">", String.Empty);
            }

            // Remove dangerous values from tags
            text = Regex.Replace(text, "javascript:", String.Empty, RegexOptions.IgnoreCase);

            return text;
        }

        /// <summary>
        /// Get the text content of an HTML string, but without text used for links
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string TextOutsideLinks(string text)
        {
            if (String.IsNullOrEmpty(text)) return text;

            // Remove any links including the link text
            var anythingExceptEndAnchor = "((?!</a>).)*";
            text = Regex.Replace(text, "<a [^>]*>" + anythingExceptEndAnchor + "</a>", String.Empty);

            // Remove any other HTML, and what's left is text outside links
            text = HttpUtility.HtmlDecode(Html.StripTags(text));

            // Any remaining text is invalid
            return text.Trim();
        }
    }
}
