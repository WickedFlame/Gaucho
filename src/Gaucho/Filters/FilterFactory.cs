
namespace Gaucho.Filters
{
    public class FilterFactory
    {
        public static IFilter CreateFilter(string input)
        {
            var propertyIndex = input?.IndexOf("->") ?? -1;
            if (propertyIndex > 0)
            {
                var source = input.Substring(0, propertyIndex).Trim();
                var destination = input.Substring(propertyIndex + 2).Trim();
                return new PropertyFilter(source, destination);
            }

            return null;
        }
    }
}
