#region using
using Cencon.Interception;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion using

namespace Cencon.Core
{
	public abstract class QueryProvider<TItemReader> : IQueryProvider<TItemReader>
	{
		readonly SimpleCall _simpleCall;
		readonly CallWithInterception _callWithInterception;
		ICall _impl;

		public QueryProvider(IDataProvider dataProvider)
		{
			_impl = _simpleCall = new SimpleCall(dataProvider);
			_callWithInterception = new CallWithInterception(dataProvider);
		}

		public IQueryable<TItemReader> CreateQuery(IQueryable<TItemReader> source, IQueryParameters parameters)
			=> new Queryable<TItemReader>(source.Provider, parameters);

		public IEnumerable<TItemReader> Execute(IQueryable<TItemReader> query)
			=> _impl.Query(CreateUrl(query), GetItemReader);

		public IEnumerableAsync<TItemReader> ExecuteAsync(IQueryable<TItemReader> query)
			=> _impl.QueryAsync(CreateUrl(query), GetItemReaderAsync);

		public Stream GetRawContent(string url)
			=> _impl.GetData(FixContentUrl(url));

		public Task<Stream> GetRawContentAsync(string url, CancellationToken cancellationToken)
			=> _impl.GetDataAsync(FixContentUrl(url), cancellationToken);

		#region interception
		public void AddInterceptor(IQueryInterceptor interceptor)
		{
			_impl = _callWithInterception;
			_callWithInterception.AddInterceptor(interceptor);
		}

		public void RemoveInterceptor(IQueryInterceptor interceptor)
		{
			if (_impl == _simpleCall)
				return;

			if (_callWithInterception.RemoveInterceptor(interceptor))
				_impl = _simpleCall;
		}
		#endregion interception

		protected abstract IEnumerable<TItemReader> GetItemReader(Stream data);
		protected abstract IEnumerableAsync<TItemReader> GetItemReaderAsync(Stream data);

		string CreateUrl(IQueryable<TItemReader> query)
		{
			StringBuilder sb = new StringBuilder(query.Parameters.Path);
			foreach (string urlParam in query.Parameters.UrlParameters)
				sb.Append(';').Append(urlParam);
			return sb.ToString();
		}

		static Uri _rawContentUrlPrefix = new Uri("censhare:///service/");
		string FixContentUrl(string url)
			=> "/" + _rawContentUrlPrefix.MakeRelativeUri(new Uri(url)).ToString();

		#region implementations
		interface ICall
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			Stream GetData(string url);
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			Task<Stream> GetDataAsync(string url, CancellationToken cancellationToken);
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			IEnumerable<TItemReader> Query(string url, Func<Stream, IEnumerable<TItemReader>> parser);
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			IEnumerableAsync<TItemReader> QueryAsync(string url, Func<Stream, IEnumerableAsync<TItemReader>> parser);
		}

		class SimpleCall : ICall
		{
			readonly IDataProvider _dataProvider;
			internal SimpleCall(IDataProvider dataProvider)
				=> _dataProvider = dataProvider;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Stream GetData(string url)
				=> _dataProvider.GetData(url);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Task<Stream> GetDataAsync(string url, CancellationToken cancellationToken)
				=> _dataProvider.GetDataAsync(url, cancellationToken);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public IEnumerable<TItemReader> Query(string url, Func<Stream, IEnumerable<TItemReader>> parser)
			{
				using (Stream data = GetData(url))
					foreach (TItemReader item in parser(data))
						yield return item;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public IEnumerableAsync<TItemReader> QueryAsync(string url, Func<Stream, IEnumerableAsync<TItemReader>> parser)
				=> new EnumerableAsync(cancellationToken => GetDataAsync(url, cancellationToken), parser);
		}

		class CallWithInterception : ICall
		{
			readonly IDataProvider _dataProvider;
			internal CallWithInterception(IDataProvider dataProvider)
				=> _dataProvider = dataProvider;

			List<IQueryInterceptor> _interceptors = new List<IQueryInterceptor>();

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Stream GetData(string url)
			{
				url = CallInterceptors_Querying(url);
				return CallInterceptors_Queryed(url, _dataProvider.GetData(url));
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public async Task<Stream> GetDataAsync(string url, CancellationToken cancellationToken)
			{
				url = CallInterceptors_Querying(url);
				return CallInterceptors_Queryed(url, await _dataProvider.GetDataAsync(url, cancellationToken));
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public IEnumerable<TItemReader> Query(string url, Func<Stream, IEnumerable<TItemReader>> parser)
			{
				IEnumerable<TItemReader> result = DoQuery(url, parser);
				CallInterceptors_Parsed(url, result);
				return result;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			IEnumerable<TItemReader> DoQuery(string url, Func<Stream, IEnumerable<TItemReader>> parser)
			{
				using (Stream data = GetData(url))
					foreach (TItemReader item in parser(data))
						yield return item;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public IEnumerableAsync<TItemReader> QueryAsync(string url, Func<Stream, IEnumerableAsync<TItemReader>> parser)
			{
				IEnumerableAsync<TItemReader> result = new EnumerableAsync(cancellationToken => GetDataAsync(url, cancellationToken), parser);
				CallInterceptors_Parsed(url, result);
				return result;
			}

			string CallInterceptors_Querying(string url)
			{
				QueryingContext context = new QueryingContext() { Url = url };
				foreach (IQueryInterceptor interceptor in _interceptors)
					interceptor.Querying(context);
				return context.Url;
			}

			Stream CallInterceptors_Queryed(string url, Stream streamData)
			{
				QueryedContext context = new QueryedContext(url) { StreamData = streamData };
				foreach (IQueryInterceptor interceptor in _interceptors)
					interceptor.Queryed(context);
				return context.StreamData;
			}

			void CallInterceptors_Parsed(string url, object result)
			{
				ParsedContext context = new ParsedContext(url, result);
				foreach (IQueryInterceptor interceptor in _interceptors)
					interceptor.Parsed(context);
			}

			internal void AddInterceptor(IQueryInterceptor interceptor) => _interceptors.Add(interceptor);
			internal bool RemoveInterceptor(IQueryInterceptor interceptor) { _interceptors.Remove(interceptor); return _interceptors.Count == 0; }
		}

		class EnumerableAsync : IEnumerableAsync<TItemReader>
		{
			readonly Func<CancellationToken, Task<Stream>> _dataProvider;
			readonly Func<Stream, IEnumerableAsync<TItemReader>> _parserProvider;

			public EnumerableAsync(Func<CancellationToken, Task<Stream>> dataProvider, Func<Stream, IEnumerableAsync<TItemReader>> parserProvider)
			{
				_dataProvider = dataProvider;
				_parserProvider = parserProvider;
			}

			public IEnumeratorAsync<TItemReader> GetEnumerator()
				=> new EnumeratorAsync(_dataProvider, _parserProvider);

			class EnumeratorAsync : IEnumeratorAsync<TItemReader>
			{
				readonly Func<CancellationToken, Task<Stream>> _dataProvider;
				readonly Func<Stream, IEnumerableAsync<TItemReader>> _parserProvider;

				public EnumeratorAsync(Func<CancellationToken, Task<Stream>> dataProvider, Func<Stream, IEnumerableAsync<TItemReader>> parserProvider)
				{
					_dataProvider = dataProvider;
					_parserProvider = parserProvider;
				}

				public void Dispose()
				{
					if (_data != null)
						_data.Dispose();
					if (_source != null)
						_source.Dispose();
				}

				bool _initialized;
				Stream _data;
				IEnumeratorAsync<TItemReader> _source;

				public TItemReader Current => _source.Current;

				public async Task<bool> MoveNextAsync(CancellationToken cancellationToken)
				{
					if (!_initialized)
					{
						_data = await _dataProvider(cancellationToken);
						_source = _parserProvider(_data).GetEnumerator();
						_initialized = true;
					}

					return await _source.MoveNextAsync(cancellationToken);
				}

				public Task Reset(CancellationToken cancellationToken)
					=> _source.Reset(cancellationToken);
			}
		}
		#endregion implementations
	}
}
