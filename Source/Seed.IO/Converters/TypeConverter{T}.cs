using System;
using System.ComponentModel;
using System.Globalization;

namespace Seed.IO.Converters
{
    /// <summary>
    /// Generic form of a <see cref="TypeConverter"/>
    /// </summary>
    /// <typeparam name="T">The type to convert</typeparam>
    internal abstract class TypeConverter<T> : TypeConverter
    {
        /// <inheritdoc cref="TypeConverter"/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <inheritdoc cref="TypeConverter"/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <inheritdoc cref="TypeConverter"/>
        public override object? ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object? rawValue, Type destinationType)
        {
            if(rawValue is T asT)
            {
                return GetValue(asT);
            }
            return null;
        }

        /// <inheritdoc cref="TypeConverter"/>
        public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object? value)
        {
            string? stringValue = value as string;
            if(stringValue == null)
            {
                return null;
            }
            return Create(stringValue);
        }

        /// <summary>
        /// Takes in a string and creates an instance of the requested type.
        /// </summary>
        /// <param name="value">The value to create the instance from.</param>
        /// <returns>The created instance</returns>
        protected abstract T Create(string value);

        /// <summary>
        /// Gets the value from the given instance 
        /// </summary>
        /// <param name="instance">The instance to get the value from</param>
        /// <returns>The string value</returns>
        protected abstract string GetValue(T instance);
    }
}
