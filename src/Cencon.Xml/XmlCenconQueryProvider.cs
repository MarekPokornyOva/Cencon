#region using
using Cencon.Core;
using System.Collections.Generic;
using System.IO;
using System.Xml;
#endregion using

namespace Cencon.Xml
{
	public class XmlCenconQueryProvider : QueryProvider<XmlReader>
	{
		public XmlCenconQueryProvider(IDataProvider dataProvider) : base(dataProvider)
		{
		}

		protected override IEnumerable<XmlReader> GetItemReader(Stream data)
		=> new XmlParser(data, true);

		protected override IEnumerableAsync<XmlReader> GetItemReaderAsync(Stream data)
			=> new XmlParser(data, true);
	}
}
