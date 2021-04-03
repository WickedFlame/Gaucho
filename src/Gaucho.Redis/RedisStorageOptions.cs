
namespace Gaucho.Redis
{
	/// <summary>
	/// Options for the RedisStorage
	/// </summary>
	public class RedisStorageOptions
	{
		/// <summary>
		/// Gets or sets the prefix used in the Redis key
		/// </summary>
		public string Prefix { get; set; } = "gaucho";

		/// <summary>
		/// Gets or sets the default database number used in Redis
		/// </summary>
		public int Db { get; set; } = 0;
	}
}
