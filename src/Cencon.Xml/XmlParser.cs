#region using
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
#endregion using

namespace Cencon.Xml
{
	class XmlParser : IParser<XmlReader>
	{
		readonly static XmlReaderSettings CommonXmlReaderSettingsSync = new XmlReaderSettings { IgnoreWhitespace = true, CloseInput = false, IgnoreComments = true, Async = false };
		readonly static XmlReaderSettings CommonXmlReaderSettingsAsync = new XmlReaderSettings { IgnoreWhitespace = true, CloseInput = false, IgnoreComments = true, Async = true };

		readonly XmlReader _reader;
		bool _enumeratorCreated;

		public XmlParser(Stream data, bool async)
		{
			_reader = XmlReader.Create(data, async ? CommonXmlReaderSettingsAsync : CommonXmlReaderSettingsSync);
		}

		void EnsureSingleEnumerator()
		{
			if (_enumeratorCreated)
				throw new NotSupportedException("Can't enumerate repeatedly.");
			_enumeratorCreated = true;
		}

		public IEnumerator<XmlReader> GetEnumerator()
		{
			EnsureSingleEnumerator();
			return new Enumerator(_reader);
		}

		IEnumerator IEnumerable.GetEnumerator()
			=> this.GetEnumerator();

		IEnumeratorAsync<XmlReader> IEnumerableAsync<XmlReader>.GetEnumerator()
		{
			EnsureSingleEnumerator();
			return new EnumeratorAsync(_reader);
		}

		class Enumerator : IEnumerator<XmlReader>
		{
			XmlReader _reader;
			XmlReader _current;

			public Enumerator(XmlReader reader)
			{
				_reader = reader;
			}

			public XmlReader Current => _current;

			object IEnumerator.Current => _current;

			public void Dispose()
			{ }

			public bool MoveNext()
			{
				while (_reader.Read())
					if (CheckAssetStart(_reader))
					{
						_current = new FragmentXmlReader(_reader);
						return true;
					}

				return false;
			}

			public void Reset()
				=> throw new NotSupportedException("Can't reset enumeration.");

			internal static bool CheckAssetStart(XmlReader reader)
				=> (reader.Depth==1)&&(reader.NodeType==XmlNodeType.Element)&&(string.Equals(reader.Name,"asset",StringComparison.Ordinal));
		}

		class EnumeratorAsync : IEnumeratorAsync<XmlReader>
		{
			XmlReader _reader;
			XmlReader _current;

			public EnumeratorAsync(XmlReader reader)
			{
				_reader = reader;
			}

			public void Dispose()
				=> _reader.Dispose();

			public XmlReader Current => _current;

			public async Task<bool> MoveNextAsync(CancellationToken cancellationToken)
			{
				while (await _reader.ReadAsync())
					if (Enumerator.CheckAssetStart(_reader))
					{
						_current = new FragmentXmlReader(_reader);
						return true;
					}

				return false;
			}

			public Task Reset(CancellationToken cancellationToken)
				=> throw new NotSupportedException("Can't reset enumeration.");
		}

		#region FragmentXmlReader
		class FragmentXmlReader : XmlReader
		{
			readonly XmlReader _reader;
			readonly int _depthZero;

			public FragmentXmlReader(XmlReader reader)
			{
				_reader = reader;
				_depthZero = _reader.Depth;
			}

			public override int AttributeCount => _reader.AttributeCount;

			public override string BaseURI => _reader.BaseURI;

			public override int Depth => _reader.Depth - _depthZero;

			public override bool EOF => _reader.EOF;

			public override bool IsEmptyElement => _reader.IsEmptyElement;

			public override string LocalName => _reader.LocalName;

			public override string NamespaceURI => _reader.NamespaceURI;

			public override XmlNameTable NameTable => _reader.NameTable;

			public override XmlNodeType NodeType => _reader.NodeType;

			public override string Prefix => _reader.Prefix;

			public override ReadState ReadState => _reader.ReadState;

			public override string Value => _reader.Value;

			public override string GetAttribute(int i) => _reader.GetAttribute(i);

			public override string GetAttribute(string name) => _reader.GetAttribute(name);

			public override string GetAttribute(string name, string namespaceURI) => _reader.GetAttribute(name, namespaceURI);

			public override string LookupNamespace(string prefix) => _reader.LookupNamespace(prefix);

			public override bool MoveToAttribute(string name) => _reader.MoveToAttribute(name);

			public override bool MoveToAttribute(string name, string ns) => _reader.MoveToAttribute(name, ns);

			public override bool MoveToElement() => Depth == 0 && _reader.MoveToElement();

			public override bool MoveToFirstAttribute() => _reader.MoveToFirstAttribute();

			public override bool MoveToNextAttribute() => _reader.MoveToNextAttribute();

			public override bool Read() => (Depth!=0 || NodeType!=XmlNodeType.EndElement) && _reader.Read();

			public override bool ReadAttributeValue() => _reader.ReadAttributeValue();

			public override void ResolveEntity() => _reader.ResolveEntity();

			public override Task<string> GetValueAsync() => _reader.GetValueAsync();

			public override Task<bool> ReadAsync() => _reader.ReadAsync();
		}
		#endregion FragmentXmlReader
	}
}
