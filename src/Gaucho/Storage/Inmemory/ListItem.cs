using System.Collections.Generic;
using System.Linq;

namespace Gaucho.Storage.Inmemory
{
	/// <summary>
	/// ListItem for the inmemory storage
	/// </summary>
	public class ListItem : IStorageItem
	{
		private readonly List<IStorageItem> _items = new List<IStorageItem>();

		/// <summary>
		/// Gets the list of items
		/// </summary>
		public IEnumerable<IStorageItem> Items => _items;

		/// <summary>
		/// Add a new value to the list
		/// </summary>
		/// <param name="value"></param>
		public void SetValue(object value)
		{
			_items.Add(new ValueItem(value));
		}

		/// <summary>
		/// Get the values from the list
		/// </summary>
		/// <returns></returns>
		public object GetValue()
		{
			return _items.Select(s => s.GetValue()).ToList();
		}
	}
}
