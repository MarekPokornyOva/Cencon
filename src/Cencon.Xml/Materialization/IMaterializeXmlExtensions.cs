#region using
using Cencon.Xml.Materialization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
#endregion using

namespace Cencon.Linq
{
	public static class IMaterializeXmlExtensions
	{
		public static IEnumerable<T> Materialize<T>(this IEnumerable<XmlReader> source, IXmlMaterializer<T> materializer)
		{
			foreach (XmlReader item in source)
				if (materializer.TryMaterialize(item, out T value))
					yield return value;
		}

		public static IEnumerableAsync<T> MaterializeAsync<T>(this IEnumerableAsync<XmlReader> source, IXmlMaterializer<T> materializer)
			=> new MaterializeAsyncEnumerable<T>(source, x => (materializer.TryMaterialize(x, out T value), value));

		#region MaterializeAsyncEnumerable
		class MaterializeAsyncEnumerable<T> : IEnumerableAsync<T>
		{
			readonly IEnumerableAsync<XmlReader> _source;
			readonly Func<XmlReader, (bool,T)> _mapFunc;

			internal MaterializeAsyncEnumerable(IEnumerableAsync<XmlReader> source, Func<XmlReader, (bool, T)> mapFunc)
			{
				_source = source;
				_mapFunc = mapFunc;
			}

			public IEnumeratorAsync<T> GetEnumerator()
				=> new Enumerator(_source.GetEnumerator(), _mapFunc);

			class Enumerator : IEnumeratorAsync<T>
			{
				readonly IEnumeratorAsync<XmlReader> _source;
				readonly Func<XmlReader, (bool success, T value)> _mapFunc;

				internal Enumerator(IEnumeratorAsync<XmlReader> source, Func<XmlReader, (bool, T)> mapFunc)
				{
					_source = source;
					_mapFunc = mapFunc;
				}

				public void Dispose()
					=> _source.Dispose();

				T _current;
				public T Current => _current;

				public async Task<bool> MoveNextAsync(CancellationToken cancellationToken)
				{
					while (await _source.MoveNextAsync(cancellationToken))
					{
						(bool success, T value) = _mapFunc(_source.Current);
						if (success)
						{
							_current = value;
							return true;
						}
					}
					return false;
				}

				public Task Reset(CancellationToken cancellationToken)
					=> _source.Reset(cancellationToken);
			}
		}
		#endregion MaterializeAsyncEnumerable
	}
}
