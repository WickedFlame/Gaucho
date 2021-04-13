
namespace Gaucho
{
	/// <summary>
	/// Represents a worker that possibly works on different threads
	/// </summary>
	public interface IWorker
	{
		/// <summary>
		/// Execute the worker
		/// </summary>
		void Execute();
	}
}
