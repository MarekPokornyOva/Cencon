#region using
using Cencon;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion using

namespace Products
{
	class MockDataProvider:IDataProvider
	{
		static byte[] _products = Encoding.UTF8.GetBytes("<assets><asset type=\"product.\" id=\"001\" id_extern=\"001\" name=\"Product 001\"/><asset type=\"product.\" id=\"002\" id_extern=\"002\" name=\"Product 002\"/></assets>");
		static byte[] _categories = Encoding.UTF8.GetBytes("<assets><asset type=\"product.category.\" id=\"101\" id_extern=\"101\" name=\"Product category 101\"><child_asset_rel child_asset=\"102\" key=\"user.product-category-hierarchy.\"/></asset><asset type=\"product.category.\" id=\"102\" id_extern=\"102\" name=\"Product category 102\"><child_asset_rel child_asset=\"001\" key=\"product.\"/><child_asset_rel child_asset=\"002\" key=\"product.\"/></asset></assets>");
		public Stream GetData(string url)
		{
			switch (url)
			{
				case "/assets/all;censhare:asset.type=product.":
					return new MemoryStream(_products);
				case "/assets/all;censhare:asset.type=product.category.":
					return new MemoryStream(_categories);
				default:
					throw new NotSupportedException();
			}
		}

		public Task<Stream> GetDataAsync(string url,CancellationToken cancellationToken)
			=> Task.FromResult(GetData(url));
	}
}
