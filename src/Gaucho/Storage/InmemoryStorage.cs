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
		public void AddToList<T>(string pipelineId, string key, T value)
		{
			var internalKey = InternalKey(pipelineId, key);
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
		public IEnumerable<T> GetList<T>(string pipelineId, string key) where T : class, new()
		{
			if (_store.ContainsKey(InternalKey(pipelineId, key)))
			{
				if (_store[InternalKey(pipelineId, key)].GetValue() is List<object> items)
				{
					return items.Cast<T>();
				}
			}

			return (IEnumerable<T>)default;
		}

		/// <inheritdoc/>
		public void RemoveRangeFromList(string pipelineId, string key, int count)
		{
			if (_store.ContainsKey(InternalKey(pipelineId, key)))
			{
				if (_store[InternalKey(pipelineId, key)].GetValue() is List<object> items)
				{
					items.RemoveRange(0, count);
				}
			}
		}

		/// <inheritdoc/>
		public void Set<T>(string pipelineId, string key, T value)
		{
			_store[InternalKey(pipelineId, key)] = new ValueItem(value);
		}

		/// <inheritdoc/>
		public T Get<T>(string pipelineId, string key)
		{
			if (_store.ContainsKey(InternalKey(pipelineId, key)))
			{
				return (T)_store[InternalKey(pipelineId, key)].GetValue();
			}

			return (T) default;
		}

		private string InternalKey(string pipelineId, string key) => $"{pipelineId}:{key}";
	}
}
