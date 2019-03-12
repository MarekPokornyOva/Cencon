#region using
using System.Collections.Generic;
using System.Xml;
using Cencon.Xml;
using Cencon;
using Cencon.Interception;
#endregion using

namespace Products
{
   public class Client : XmlCenconClient
	{
		#region create
		Client(IQueryProvider<XmlReader> queryProvider) : base(queryProvider)
		{}

		public static Client Create(IDataProvider dataProvider,IEnumerable<IQueryInterceptor> interceptors)
		{
			XmlCenconQueryProvider provider = new XmlCenconQueryProvider(dataProvider);
			foreach (IQueryInterceptor interceptor in interceptors)
				provider.AddInterceptor(interceptor);
			return new Client(provider);
		}
		#endregion create

		#region Products
		public IEnumerable<ImportProduct> Products()
			=> Query().AssetsAll().AssetType("product.").AsProducts();

		public IEnumerableAsync<ImportProduct> ProductsAsync()
			=> Query().AssetsAll().AssetType("product.").AsProductsAsync();
		#endregion Products

		#region ProductCategories
		public IEnumerable<ImportProductCategory> ProductCategories()
			=> Query().AssetsAll().AssetType("product.category.").AsProductCategories();

		public IEnumerableAsync<ImportProductCategory> ProductCategoriesAsync()
			=> Query().AssetsAll().AssetType("product.category.").AsProductCategoriesAsync();
		#endregion ProductCategories
	}
}
