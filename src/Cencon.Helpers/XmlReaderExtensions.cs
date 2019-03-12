#region using
using System;
using System.Globalization;
using System.Xml;
#endregion using

namespace Cencon
{
	public static class XmlReaderExtensions
	{
		public static string GetAttributeRequired(this XmlReader reader, string name)
		{
			string result = reader.GetAttribute(name);
			if (string.IsNullOrEmpty(result))
				throw new CenconStructureException($"Element's does not contain \"{name}\" attribute or its value is blank.");
			return result;
		}

		public static int? GetAttributeAsNullableInt(this XmlReader reader, string attrName)
		{
			string temp = reader.GetAttribute(attrName);
			return temp == null ? (int?)null : int.Parse(temp);
		}

		public static int GetAttributeAsInt(this XmlReader reader, string attrName)
			=> int.Parse(reader.GetAttribute(attrName));

		public static DateTime GetAttributeAsDateTime(this XmlReader reader, string attrName)
			=> DateTime.Parse(reader.GetAttribute(attrName)).ToUniversalTime();

		public static IFormatProvider EnUsFormatProvider = CultureInfo.GetCultureInfo("en-US").NumberFormat;

		public static T CreateItem<T>(XmlReader reader) where T : Asset, new()
			=> new T { Id = reader.GetAttributeRequired("id"), IdExtern = reader.GetAttributeRequired("id_extern"), Name = reader.GetAttributeRequired("name") };

		public static T CreateItemWithCheck<T>(XmlReader reader, string expectedType) where T : Asset, new()
		{
			string type = reader.GetAttribute("type");
			if (type != expectedType)
				throw new CenconStructureException($"Invalid data: \"asset\" element is of unexpected type.");
			return CreateItem<T>(reader);
		}
	}
}
