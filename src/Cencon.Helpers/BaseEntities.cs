#region using
using System.Collections;
using System.Collections.Generic;
#endregion using

namespace Cencon
{
	public class Asset
	{
		public string Id { get; set; }
		public string IdExtern { get; set; }
		public string Name { get; set; }
	}

	public class AssetRelation
	{
		public string RelatedId { get; set; }
		public string Key { get; set; }
		public int? Sort { get; set; }
	}

	public class Content : Asset
	{
		public ItemCollection<ContentItem> Items { get; } = new ItemCollection<ContentItem>();
	}

	public class ContentItem
	{
		public string Key { get; set; }
		public string MimeType { get; set; }
		public string Url { get; set; }
	}

	#region help classes
	public class ItemCollection<T> : IReadOnlyCollection<T>
	{
		List<T> _items = new List<T>();
		public ItemCollection()
		{ }

		public int Count => _items.Count;

		public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

		public void Add(T item) => _items.Add(item);
	}
	#endregion help classes
}
