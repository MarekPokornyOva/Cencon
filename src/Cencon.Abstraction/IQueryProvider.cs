#region using
using Cencon.Interception;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion using

namespace Cencon
{
	public interface IQueryProvider<TItemReader>
	{
		IQueryable<TItemReader> CreateQuery(IQueryable<TItemReader> source, IQueryParameters parameters);

		IEnumerable<TItemReader> Execute(IQueryable<TItemReader> query);
		IEnumerableAsync<TItemReader> ExecuteAsync(IQueryable<TItemReader> query);

		Stream GetRawContent(string url);
		Task<Stream> GetRawContentAsync(string url, CancellationToken cancellationToken);

		void AddInterceptor(IQueryInterceptor interceptor);
		void RemoveInterceptor(IQueryInterceptor interceptor);
	}
}
