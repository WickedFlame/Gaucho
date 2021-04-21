using System.Collections.Generic;
using System.Linq;

namespace Gaucho.Storage.Inmemory
{
	/// <summary>
	/// ListItem for the inmemory storage
	/// </summary>
	public class ListItem : IStorageItem
	{
		/// <summary>
		/// Gets the list of items
		/// </summary>
		public List<IStorageItem> Items { get; } = new List<IStorageItem>();

		/// <summary>
		/// Add a new value to the list
		/// </summary>
		/// <param name="value"></param>
		public void SetValue(object value)
		{
			Items.Add(new ValueItem(value));
		}

		/// <summary>
		/// Get the values from the list
		/// </summary>
		/// <returns></returns>
		public object GetValue()
		{
			return Items.Select(s => s.GetValue()).ToList();
		}
	}
}
