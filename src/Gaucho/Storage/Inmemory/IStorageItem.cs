namespace Gaucho.Storage.Inmemory
{
	public interface IStorageItem
	{
		void SetValue(object value);

		object GetValue();
	}
}
