#region using
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion using

namespace Cencon
{
	public interface IClient<TItemReader>
	{
		IQueryable<TItemReader> Query();

		Stream GetRawContent(string url);

		Task<Stream> GetRawContentAsync(string url, CancellationToken cancellationToken);
	}
}
