#region using
using System.Collections.Generic;
#endregion using

namespace Cencon.Core
{
	class QueryParameters : IQueryParameters
	{
		//public int Skip { get; internal set; }

		public string Path { get; set; }

		IList<string> _urlParameters;
		public IList<string> UrlParameters { get { return _urlParameters ?? (_urlParameters = new List<string>()); } private set { _urlParameters = value; } }

		internal static QueryParameters CloneFrom(IQueryParameters source)
			=> new QueryParameters() { /*Skip = source.Skip, */ Path=source.Path, UrlParameters = new List<string>(source.UrlParameters) };
	}
}
