#region using
using System.Xml;
#endregion using

namespace Cencon.Xml.Materialization
{
	public interface IXmlMaterializer<T>
	{
		bool TryMaterialize(XmlReader source, out T result);
	}
}
