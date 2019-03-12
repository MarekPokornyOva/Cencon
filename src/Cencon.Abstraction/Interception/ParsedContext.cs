#region using
using System.IO;
#endregion using

namespace Cencon.Interception
{
	public class ParsedContext
	{
		public ParsedContext(string url, object parsedResult)
		{
			Url = url;
			ParsedResult = parsedResult;
		}

		public string Url { get; }
		public object ParsedResult { get; }
	}
}
