#region using
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#endregion using

namespace System.Linq
{
	public static class EnumerableAsyncExtensions
	{
		public static IEnumerableAsync<TResult> OfType<TSource,TResult>(this IEnumerableAsync<TSource> source)
			=> new FuncEnumerable<TSource, TResult>(source, x=>(TResult)(object)x);

		#region FuncEnumerable
		class FuncEnumerable<TSource, TResult> : IEnumerableAsync<TResult>
		{
			IEnumerableAsync<TSource> _source;
			readonly Func<TSource, TResult> _func;

			internal FuncEnumerable(IEnumerableAsync<TSource> source, Func<TSource, TResult> func)
			{
				_source = source;
				_func = func;
			}

			public IEnumeratorAsync<TResult> GetEnumerator()
				=> new FuncEnumerator<TSource,TResult>(_source.GetEnumerator(), _func);
		}

		internal class FuncEnumerator<TSource,TResult> : IEnumeratorAsync<TResult>
		{
			readonly IEnumeratorAsync<TSource> _source;
			readonly Func<TSource, TResult> _func;

			public FuncEnumerator(IEnumeratorAsync<TSource> source, Func<TSource, TResult> func)
			{
				_source = source;
				_func = func;
			}

			public void Dispose()
				=> _source.Dispose();

			public TResult Current => _func(_source.Current);

			public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
				=> _source.MoveNextAsync(cancellationToken);

			public Task Reset(CancellationToken cancellationToken)
				=> _source.Reset(cancellationToken);
		}
		#endregion FuncEnumerable

		#region WhereEnumerable
		class WhereEnumerable<TSource> : IEnumerableAsync<TSource>
		{
			IEnumerableAsync<TSource> _source;
			readonly Func<TSource, bool> _predicate;

			internal WhereEnumerable(IEnumerableAsync<TSource> source, Func<TSource, bool> predicate)
			{
				_source = source;
				_predicate = predicate;
			}

			public IEnumeratorAsync<TSource> GetEnumerator()
				=> new WhereEnumerator<TSource>(_source.GetEnumerator(), _predicate);
		}

		internal class WhereEnumerator<TSource> : IEnumeratorAsync<TSource>
		{
			readonly IEnumeratorAsync<TSource> _source;
			readonly Func<TSource, bool> _predicate;

			public WhereEnumerator(IEnumeratorAsync<TSource> source, Func<TSource, bool> predicate)
			{
				_source = source;
				_predicate = predicate;
			}

			public void Dispose()
				=> _source.Dispose();

			public TSource Current => _source.Current;

			public async Task<bool> MoveNextAsync(CancellationToken cancellationToken)
			{
				bool result;
				while ((result=await _source.MoveNextAsync(cancellationToken)) && !_predicate(_source.Current)) ;
				return result;
			}

			public Task Reset(CancellationToken cancellationToken)
				=> _source.Reset(cancellationToken);
		}
		#endregion WhereEnumerable

		public static async Task<T> FirstOrDefaultAsync<T>(this IEnumerableAsync<T> source, CancellationToken cancellationToken)
		{
			using (IEnumeratorAsync<T> en = source.GetEnumerator())
				return await en.MoveNextAsync(cancellationToken) ? en.Current : default(T);
		}

		public static IEnumerableAsync<TResult> Select<TSource, TResult>(this IEnumerableAsync<TSource> source, Func<TSource, TResult> selector)
			=> new FuncEnumerable<TSource, TResult>(source, selector);

		public static async Task ForEachAsync<TSource>(this IEnumerableAsync<TSource> source, Action<TSource> iterationAction, CancellationToken cancellationToken)
		{
			using (IEnumeratorAsync<TSource> en = source.GetEnumerator())
				while (await en.MoveNextAsync(cancellationToken))
					iterationAction(en.Current);
		}

		public static async Task ForEachAsync<TSource>(this IEnumerableAsync<TSource> source, Func<TSource,Task> iterationAction, CancellationToken cancellationToken)
		{
			using (IEnumeratorAsync<TSource> en = source.GetEnumerator())
				while (await en.MoveNextAsync(cancellationToken))
					await iterationAction(en.Current);
		}

		public static IEnumerableAsync<TSource> Where<TSource>(this IEnumerableAsync<TSource> source, Func<TSource, bool> predicate)
			=> new WhereEnumerable<TSource>(source, predicate);

		public static async Task<int> CountAsync<TSource>(this IEnumerableAsync<TSource> source, CancellationToken cancellationToken)
		{
			int count = 0;
			await ForEachAsync(source, x => count++, cancellationToken);
			return count;
		}
	}
}
