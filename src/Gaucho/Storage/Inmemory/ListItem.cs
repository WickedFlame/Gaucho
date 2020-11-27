using System.Collections.Generic;
using System.Linq;

namespace Gaucho.Storage.Inmemory
{
	public class ListItem : IStorageItem
	{
		private readonly List<IStorageItem> _items = new List<IStorageItem>();

		public IEnumerable<IStorageItem> Items => _items;

		public void SetValue(object value)
		{
			_items.Add(new ValueItem(value));
		}

		public object GetValue()
		{
			return _items.Select(s => s.GetValue()).ToList();
		}
	}
}
