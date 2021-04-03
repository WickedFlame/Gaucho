using System.Collections.Generic;

namespace Gaucho.Storage
{
	/// <summary>
	/// Interface for the storage
	/// </summary>
	public interface IStorage
	{
		/// <summary>
		/// Add a value to a list in the storage
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pipelineId"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void AddToList<T>(string pipelineId, string key, T value);

		/// <summary>
		/// Get a list of values from the storage
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pipelineId"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		IEnumerable<T> GetList<T>(string pipelineId, string key) where T : class, new();

		/// <summary>
		/// Removes a range of items from the list
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="key"></param>
		/// <param name="count"></param>
		void RemoveRangeFromList(string pipelineId, string key, int count);

		/// <summary>
		/// Set a value to the storage
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pipelineId"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void Set<T>(string pipelineId, string key, T value);

		/// <summary>
		/// Gets a value from the storage
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pipelineId"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		T Get<T>(string pipelineId, string key);
	}
}
