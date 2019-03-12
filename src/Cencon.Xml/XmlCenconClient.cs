#region using
using Cencon.Core;
using System.Xml;
#endregion using

namespace Cencon.Xml
{
	public class XmlCenconClient : Client<XmlReader>
	{
		public XmlCenconClient(IQueryProvider<XmlReader> queryProvider) : base(queryProvider)
		{
		}
	}
}
