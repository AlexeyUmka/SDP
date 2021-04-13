using System.Text;

namespace BrainstormSessions.Helpers
{
    public static class ObjectHelper
    {
        public static string GetPropertiesAndFieldsStringRepresentation(this object obj)
        {
            var type = obj.GetType();
            var properties = type.GetProperties();
            var fields = type.GetFields();
            var result = new StringBuilder();
            foreach (var property in properties)
            {
                result.Append($"{property.Name}:{property.GetValue(obj)};");
            }
            foreach (var field in fields)
            {
                result.Append($"{field.Name}:{field.GetValue(obj)};");
            }

            return result.ToString();
        }
    }
}