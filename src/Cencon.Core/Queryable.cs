#region using
using System.Collections;
using System.Collections.Generic;
#endregion using

namespace Cencon.Core
{
	class Queryable<TItemReader> : IQueryable<TItemReader>
	{
		readonly IQueryProvider<TItemReader> _provider;
		readonly IQueryParameters _parameters;

		internal Queryable(IQueryProvider<TItemReader> provider, IQueryParameters parameters)
		{
			_provider = provider;
			_parameters = parameters;
		}

		public IQueryParameters Parameters => _parameters;

		public IQueryProvider<TItemReader> Provider => _provider;

		public IEnumerator<TItemReader> GetEnumerator()
			=> _provider.Execute(this).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> this.GetEnumerator();

		IEnumeratorAsync<TItemReader> IEnumerableAsync<TItemReader>.GetEnumerator()
			=> _provider.ExecuteAsync(this).GetEnumerator();
	}
}
