namespace Gaucho.Storage
{
	public interface IStorage
	{
		void Add<T>(string pipelineId, string key, T value);

		void Set<T>(string pipelineId, string key, T value);

		T Get<T>(string pipelineId, string key);
	}
}
