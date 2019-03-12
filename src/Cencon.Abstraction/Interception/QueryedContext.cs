#region using
using System.IO;
#endregion using

namespace Cencon.Interception
{
	public class QueryedContext
	{
		public QueryedContext(string url)
		{
			Url = url;
		}

		public string Url { get; }

		public Stream StreamData { get; set; }
	}
}
