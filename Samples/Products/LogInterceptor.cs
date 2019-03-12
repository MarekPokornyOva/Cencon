#region using
using Cencon.Interception;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion using

namespace Products
{
	class LogInterceptor : IQueryInterceptor
	{
		readonly Action<string> _queryingCallback;
		readonly Action<int> _dataRetrievedCallback;

		internal LogInterceptor(Action<string> queryingCallback, Action<int> dataRetrievedCallback)
		{
			_queryingCallback = queryingCallback;
			_dataRetrievedCallback = dataRetrievedCallback;
		}

		public void Parsed(ParsedContext context)
		{
		}

		LogStream _logStream;
		public void Queryed(QueryedContext context)
		{
			context.StreamData = _logStream = new LogStream(context.StreamData, _dataRetrievedCallback);
		}

		public void Querying(QueryingContext context)
			=> _queryingCallback(context.Url);
	}

	class LogStream : Stream
	{
		readonly Stream _source;
		private int _dataRetrieved;
		readonly Action<int> _dataRetrievedCallback;

		public LogStream(Stream source, Action<int> dataRetrievedCallback)
		{
			_source = source;
			_dataRetrievedCallback = dataRetrievedCallback;
		}

		protected override void Dispose(bool disposing)
		{
			_dataRetrievedCallback(_dataRetrieved);
			_source.Dispose();
		}

		public override bool CanRead => _source.CanRead;

		public override bool CanSeek => _source.CanSeek;

		public override bool CanWrite => _source.CanWrite;

		public override long Length => _source.Length;

		public override long Position { get => _source.Position; set => _source.Position = value; }

		public override void Flush()
			=> _source.Flush();

		public override int Read(byte[] buffer, int offset, int count)
		{
			int read = _source.Read(buffer, offset, count);
			_dataRetrieved += read;
			return read;
		}

		public override long Seek(long offset, SeekOrigin origin)
			=> _source.Seek(offset, origin);

		public override void SetLength(long value)
			=> _source.SetLength(value);

		public override void Write(byte[] buffer, int offset, int count)
			=> _source.Write(buffer, offset, count);

		public override async Task<int> ReadAsync(byte[] buffer,int offset,int count,CancellationToken cancellationToken)
		{
			int read = await _source.ReadAsync(buffer,offset,count,cancellationToken);
			_dataRetrieved+=read;
			return read;
		}
	}
}
