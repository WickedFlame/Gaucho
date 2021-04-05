using System;
using System.Collections.Generic;
using System.Linq;
using Gaucho.Storage.Inmemory;

namespace Gaucho.Storage
{
	/// <summary>
	/// The default Storage
	/// </summary>
	public class InmemoryStorage : IStorage
	{
		private readonly Dictionary<string, IStorageItem> _store = new Dictionary<string, IStorageItem>();

		/// <inheritdoc/>
		public void AddToList<T>(StorageKey key, T value)
		{
			var internalKey = CreateKey(key);
			if (!_store.ContainsKey(internalKey))
			{
				_store.Add(internalKey, new ListItem());
			}

			var lst = _store[internalKey] as ListItem;
			if (lst == null)
			{
				throw new InvalidOperationException($"{internalKey} is not a list");
			}

			lst.SetValue(value);
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetList<T>(StorageKey key) where T : class, new()
		{
			if (_store.ContainsKey(CreateKey(key)))
			{
				if (_store[CreateKey(key)].GetValue() is List<object> items)
				{
					return items.Cast<T>();
				}
			}

			return (IEnumerable<T>)default;
		}

		/// <inheritdoc/>
		public void RemoveRangeFromList(StorageKey key, int count)
		{
			if (_store.ContainsKey(CreateKey(key)))
			{
				if (_store[CreateKey(key)].GetValue() is List<object> items)
				{
					items.RemoveRange(0, count);
				}
			}
		}

		/// <inheritdoc/>
		public void Set<T>(StorageKey key, T value)
		{
			_store[CreateKey(key)] = new ValueItem(value);
		}

		/// <inheritdoc/>
		public T Get<T>(StorageKey key)
		{
			if (_store.ContainsKey(CreateKey(key)))
			{
				return (T)_store[CreateKey(key)].GetValue();
			}

			return (T) default;
		}

		private string CreateKey(StorageKey key) => $"{key.PipelineId}:{key.Key}";
	}
}
