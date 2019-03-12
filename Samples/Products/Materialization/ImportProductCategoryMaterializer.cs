#region using
using Cencon;
using Cencon.Xml.Materialization;
using System.Xml;
#endregion using

namespace Products
{
	class ImportProductCategoryMaterializer : IXmlMaterializer<ImportProductCategory>
	{
		public bool TryMaterialize(XmlReader reader, out ImportProductCategory result)
		{
			result = XmlReaderExtensions.CreateItemWithCheck<ImportProductCategory>(reader, "product.category.");
			while (reader.Read())
				if ((reader.NodeType == XmlNodeType.Element) && (reader.Depth == 1))
					switch (reader.Name)
					{
						case "child_asset_rel":
							result.ChildrenItemsInternal.Add(new AssetRelation { RelatedId = reader.GetAttribute("child_asset"), Key = reader.GetAttribute("key"), Sort = reader.GetAttributeAsNullableInt("sorting") });
							break;
					}
					return true;
		}
	}
}
