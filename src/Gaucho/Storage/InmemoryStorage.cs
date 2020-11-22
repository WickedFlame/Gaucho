using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Storage
{
	public class InmemoryStorage : IStorage
	{
		public void Add<T>(string pipelineId, string key, T value)
		{
			throw new NotImplementedException();
		}

		public void Set<T>(string pipelineId, string key, T value)
		{
			throw new NotImplementedException();
		}

		public T Get<T>(string pipelineId, string key)
		{
			throw new NotImplementedException();
		}
	}
}
