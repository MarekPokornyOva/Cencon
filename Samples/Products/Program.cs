#region using
using Cencon;
using System;
using System.Linq;
using System.Threading.Tasks;
#endregion using

namespace Products
{
	class Program
	{
		static void Main(string[] args)
		{
			LogInterceptor logInterceptor = new LogInterceptor(
				query => Console.WriteLine("Querying: " + query),
				bytesCount =>
				{
					Console.WriteLine("-------------------");
					Console.WriteLine("Data retrieved: " + bytesCount);
				});
			Client client = CreateClientMock(logInterceptor);

			MainProducts(client);
			MainCategories(client);
			
			/*Task.Run(async ()=> 
			{
				await MainProductsAsync(client);
				await MainCategoriesAsync(client);
			}).GetAwaiter().GetResult();*/
		}

		static Client CreateClientMock(LogInterceptor logInterceptor)
		{
			Console.WriteLine("This demo uses mock data on current setup. But it might connect to a real service using proper data provider.");
			Console.WriteLine();

			IDataProvider dataProvider = new MockDataProvider();
			return Client.Create(dataProvider,new[] { logInterceptor });
		}

		static void MainProducts(Client client)
		{
			Console.WriteLine("Enlisting products");
			foreach (ImportProduct item in client.Products())
				Console.WriteLine(item.Name);
			Console.WriteLine("===================");
		}

		static void MainCategories(Client client)
		{
			Console.WriteLine("Enlisting product categories");
			foreach (ImportProductCategory item in client.ProductCategories())
				Console.WriteLine($"{item.Id} - {item.Name} - {item.GetChildren().Count()} children; {item.GetProducts().Count()} products");
			Console.WriteLine("===================");
		}

		static async Task MainProductsAsync(Client client)
		{
			Console.WriteLine("Enlisting products");
			await client.ProductsAsync().ForEachAsync(item => Console.WriteLine(item.Name),System.Threading.CancellationToken.None);
			Console.WriteLine("===================");
		}

		static async Task MainCategoriesAsync(Client client)
		{
			Console.WriteLine("Enlisting product categories");
			await client.ProductCategoriesAsync().ForEachAsync(item => Console.WriteLine($"{item.Id} - {item.Name} - {item.GetChildren().Count()} children; {item.GetProducts().Count()} products"),System.Threading.CancellationToken.None);
			Console.WriteLine("===================");
		}
	}
}
