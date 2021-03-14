using DeXcor.Services;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace DeXcor.Helpers
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public Type EnumType { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                if (!Enum.IsDefined(EnumType, value))
                {
                    throw new ArgumentException("value must be an Enum!");
                }

                var enumValue = Enum.Parse(EnumType, enumString);

                return enumValue.Equals(value);
            }

            throw new ArgumentException("parameter must be an Enum name!");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                return Enum.Parse(EnumType, enumString);
            }

            throw new ArgumentException("parameter must be an Enum name!");
        }
    }
    public class EmojiConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ImageDataService.PhotoCatalogCollection.FirstOrDefault(x => x.PhotoType.Equals(value))?.Emoji;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException("parameter must be an Enum name!");
        }
    }
}
