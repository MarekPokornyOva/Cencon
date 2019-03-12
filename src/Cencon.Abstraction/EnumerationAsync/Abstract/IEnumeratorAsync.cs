#region using
using System.Threading;
using System.Threading.Tasks;
#endregion using

namespace System.Collections.Generic
{
	public interface IEnumeratorAsync<out T> : IDisposable
	{
		T Current { get; }
		Task<bool> MoveNextAsync(CancellationToken cancellationToken);
		Task Reset(CancellationToken cancellationToken);
	}
}
