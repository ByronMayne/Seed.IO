using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Seed.IO.Converters
{
    internal class AbsolutePathTypeConverter : TypeConverter<AbsolutePath>
    {
        /// <inheritdoc cref="TypeConverter{T}"/>
        protected override AbsolutePath Create(string value)
            => new AbsolutePath(value);

        /// <inheritdoc cref="TypeConverter{T}"/>
        protected override string GetValue(AbsolutePath instance)
            => instance.Value;
    }
}
