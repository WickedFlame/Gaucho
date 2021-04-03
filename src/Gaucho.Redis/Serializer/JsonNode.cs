namespace Gaucho.Redis.Serializer
{
	/// <summary>
	/// Node for a json object
	/// </summary>
	public class JsonNode
	{
		/// <summary>
		/// Gets or sets the propertyname
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// Gets or sets the value
		/// </summary>
		public string Value { get; set; }
	}
}
