#region using
using Cencon.Linq;
using System.Collections.Generic;
using System.Xml;
#endregion using

namespace Products
{
	public static class ICenconQueryableMaterializeExtensions
	{
		readonly static ImportProductMaterializer _importProductMaterializer = new ImportProductMaterializer();
		readonly static ImportProductCategoryMaterializer _importProductCategoryMaterializer = new ImportProductCategoryMaterializer();

		public static IEnumerable<ImportProduct> AsProducts(this IEnumerable<XmlReader> source)
			=> source.Materialize(_importProductMaterializer);

		public static IEnumerableAsync<ImportProduct> AsProductsAsync(this IEnumerableAsync<XmlReader> source)
			=> source.MaterializeAsync(_importProductMaterializer);

		public static IEnumerable<ImportProductCategory> AsProductCategories(this IEnumerable<XmlReader> source)
			=> source.Materialize(_importProductCategoryMaterializer);

		public static IEnumerableAsync<ImportProductCategory> AsProductCategoriesAsync(this IEnumerableAsync<XmlReader> source)
			=> source.MaterializeAsync(_importProductCategoryMaterializer);
	}
}
