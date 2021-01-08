using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Storage.Inmemory;

namespace Gaucho.Storage
{
	public class InmemoryStorage : IStorage
	{
		private readonly Dictionary<string, IStorageItem> _store = new Dictionary<string, IStorageItem>();

		public void Add<T>(string pipelineId, string key, T value)
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

		public void Set<T>(string pipelineId, string key, T value)
		{
			_store[InternalKey(pipelineId, key)] = new ValueItem(value);
		}

		public T Get<T>(string pipelineId, string key)
		{
			if (_store.ContainsKey(InternalKey(pipelineId, key)))
			{
				return (T)_store[InternalKey(pipelineId, key)].GetValue();
			}

			return (T) default;
		}

		public IEnumerable<T> GetList<T>(string pipelineId, string key)
		{
			if (_store.ContainsKey(InternalKey(pipelineId, key)))
			{
				if(_store[InternalKey(pipelineId, key)].GetValue() is List<object> items)
				{
					return items.Cast<T>();
				}
			}

			return (IEnumerable<T>)default;
		}

		private string InternalKey(string pipelineId, string key) => $"{pipelineId}:{key}";
	}
}
