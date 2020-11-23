namespace Gaucho.Storage.Inmemory
{
	public class ValueItem : IStorageItem
	{
		public ValueItem()
		{
		}

		public ValueItem(object value)
		{
			Value = value;
		}

		public object Value { get; private set; }

		public void SetValue(object value)
		{
			Value = value;
		}

		public object GetValue()
		{
			return Value;
		}
	}
}
