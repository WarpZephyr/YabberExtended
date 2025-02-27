using System;
using System.Collections.Generic;
using System.IO;

namespace YabberExtended.Helpers
{
    internal static class EnumCache<TUnder, TEnum>
        where TUnder : unmanaged, IEquatable<TUnder>
        where TEnum : struct, Enum
    {
        static readonly string[] Names = Enum.GetNames<TEnum>();
        static readonly TEnum[] Values = Enum.GetValues<TEnum>();
        static readonly Dictionary<TEnum, int> IndexDictionary = BuildIndexDictionary();

        static EnumCache()
        {
            if (Enum.GetUnderlyingType(typeof(TEnum)) != typeof(TUnder))
            {
                throw new InvalidOperationException($"Underlying type does not match enum underlying type: {Enum.GetUnderlyingType(typeof(TEnum)).Name} != {typeof(TUnder).Name}");
            }
        }

        /// <summary>
        /// Assert a value is present in an <see cref="Enum"/> type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueFormat">The formatting to use on the value in errors.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidDataException">The value was not present in the <see cref="Enum"/>.</exception>
        private static TEnum AssertEnum(TUnder value, string valueFormat)
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                throw new InvalidDataException($"Read value not present in enum: {string.Format(valueFormat, value)}");
            }
            return (TEnum)(object)value;
        }

        public static string[] GetEnumNames()
            => Names;

        public static string GetEnumName(TEnum value)
            => Names[GetEnumIndex(value)];

        public static string GetEnumName(int index)
            => Names[index];

        public static int GetEnumIndex(TEnum value)
            => IndexDictionary[value];

        public static TEnum GetEnumValue(TUnder value)
            => AssertEnum(value, "0x{0:X}");

        public static TEnum GetEnumValue(int index)
            => Values[index];

        public static TUnder GetUnderlyingValue(TEnum value)
            => (TUnder)(object)value;

        public static TUnder GetUnderlyingValue(int index)
            => (TUnder)(object)GetEnumValue(index);

        static Dictionary<TEnum, int> BuildIndexDictionary()
        {
            var indexDictionary = new Dictionary<TEnum, int>(Values.Length);
            for (int i = 0; i < Values.Length; i++)
            {
                indexDictionary.Add(Values[i], i);
            }

            return indexDictionary;
        }
    }
}
