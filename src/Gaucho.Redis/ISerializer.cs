
namespace Gaucho.Redis
{
	/// <summary>
	/// Interface used for serializing objects to string
	/// </summary>
	public interface ISerializer
	{
		/// <summary>
		/// Serialize the object to a json string
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		string Serialize(object item);

		/// <summary>
		/// Deserialize a json string to an object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="json"></param>
		/// <returns></returns>
		T Deserialize<T>(string json) where T : class, new();
	}
}
