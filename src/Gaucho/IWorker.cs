
namespace Gaucho
{
	/// <summary>
	/// Represents a worker that possibly works on different threads
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IWorker<in T>
	{
		/// <summary>
		/// Execute the worker
		/// </summary>
		/// <param name="input"></param>
		void Execute(T input);
	}
}
