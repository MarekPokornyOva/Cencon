#region using
using Cencon;
using Cencon.Xml.Materialization;
using System.Xml;
#endregion using

namespace Products
{
	class ImportProductMaterializer : IXmlMaterializer<ImportProduct>
	{
		public bool TryMaterialize(XmlReader reader, out ImportProduct result)
		{
			result = XmlReaderExtensions.CreateItemWithCheck<ImportProduct>(reader, "product.");
			return true;
		}
	}
}
