using System;
using System.ComponentModel;
using System.Globalization;

namespace HiPets.Domain.Helpers.Utils
{
    public static class EventTypeExtension
    {
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            string resDescription = null;

            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (descriptionAttributes.Length > 0)
                        {
                            // we're only getting the first description we find
                            // others will be ignored
                            var description = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                            
                            resDescription = description;
                        }

                        break;
                    }
                }
            }
            return resDescription;
        }

        public static string GetRawDescription<T>(this T e) where T : IConvertible
        {
            string resDescription = null;

            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                        if (descriptionAttributes.Length > 0)
                        {
                            // we're only getting the first description we find
                            // others will be ignored
                            resDescription = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                        }

                        break;
                    }
                }
            }

            return resDescription;
        }
    }
}
