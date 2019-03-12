#region using
using Cencon;
using System.Collections.Generic;
using System.Linq;
#endregion using

namespace Products
{
	public class ImportProductCategory : Asset
	{
		#region internal
		internal ICollection<AssetRelation> ChildrenItemsInternal { get; } = new List<AssetRelation>();
		#endregion internal

		public IEnumerable<AssetRelation> GetChildren()
			=> ChildrenItemsInternal.Where(x => x.Key == "user.product-category-hierarchy.");

		public IEnumerable<AssetRelation> GetProducts()
			=> ChildrenItemsInternal.Where(x => x.Key=="product.");
	}
}
