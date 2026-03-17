using System.ComponentModel.DataAnnotations;

namespace Mobile.Utilities
{
    public static class EnumUtility
    {
        //Used when the Enum has _ or other symbols needing to be removed for display purposes
        public static string GetDisplayName(this Enum enumValue)
        {
            var attribute = enumValue.GetType()
                .GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            return attribute?.GetName() ?? enumValue.ToString();
        }
    }
}
