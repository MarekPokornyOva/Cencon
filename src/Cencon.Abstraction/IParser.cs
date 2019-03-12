#region using
using System.Collections.Generic;
#endregion using

namespace Cencon
{
	public interface IParser<TItemReader> : IEnumerable<TItemReader>, IEnumerableAsync<TItemReader>
	{ }
}
