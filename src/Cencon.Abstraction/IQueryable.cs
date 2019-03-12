#region using
using System.Collections.Generic;
#endregion using

namespace Cencon
{
	public interface IQueryable<TItemReader> : IEnumerable<TItemReader>, IEnumerableAsync<TItemReader>
	{
		IQueryParameters Parameters { get; }
		IQueryProvider<TItemReader> Provider { get; }
	}
}
