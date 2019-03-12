#region using
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion using

namespace Cencon.Core
{
	public class Client<TItemReader> : IClient<TItemReader>
	{
		readonly IQueryProvider<TItemReader> _queryProvider;
		public Client(IQueryProvider<TItemReader> queryProvider)
		{
			_queryProvider = queryProvider;
		}

		public IQueryable<TItemReader> Query()
			=> new Queryable<TItemReader>(_queryProvider, new QueryParameters());

		public Stream GetRawContent(string url)
			=> _queryProvider.GetRawContent(url);

		public Task<Stream> GetRawContentAsync(string url, CancellationToken cancellationToken)
			=> _queryProvider.GetRawContentAsync(url, cancellationToken);
	}
}
