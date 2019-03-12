#region using
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion using

namespace Cencon
{
	public interface IDataProvider
	{
		Stream GetData(string url);
		Task<Stream> GetDataAsync(string url, CancellationToken cancellationToken);
	}
}
