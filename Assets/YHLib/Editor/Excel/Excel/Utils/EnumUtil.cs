using System;

namespace TK.Excel
{
    public class EnumUtil
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T ParseEnum<T>(string value, T defaultValue)
        {
            if (Enum.IsDefined(typeof(T), value) && !string.IsNullOrEmpty(value))
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }

            return defaultValue;
        }
    }
}