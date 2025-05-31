using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using YabberExtended.Extensions.Value;

namespace YabberExtended.Extensions.Xml
{
    public static class XmlAttributeCollectionExtensions
    {
        public static XmlAttribute GetAttributeOrThrow(this XmlAttributeCollection attributes, string name)
            => attributes[name] ?? throw new InvalidDataException($"Attribute not found: {name}");

        #region Generic

        public static TValue ReadValue<TValue>(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null) where TValue : IParsable<TValue>
            => attributes.GetAttributeOrThrow(name).InnerText.ToValue<TValue>(name, provider);

        [return: NotNullIfNotNull(nameof(defaultValue))]
        public static TValue? ReadValueOrDefault<TValue>(this XmlAttributeCollection attributes, string name, TValue? defaultValue = default, IFormatProvider? provider = null) where TValue : IParsable<TValue>
        {
            XmlAttribute? child = attributes[name];
            return child == null ? defaultValue : child.InnerText.ToValue<TValue>(name, provider);
        }

        public static TValue? ReadValueIfExists<TValue>(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null) where TValue : struct, IParsable<TValue>
            => attributes[name]?.InnerText.ToValue<TValue>(name, provider);

        public static TEnum ReadEnum<TEnum>(this XmlAttributeCollection attributes, string name) where TEnum : struct
            => attributes.GetAttributeOrThrow(name).InnerText.ToEnum<TEnum>(name);

        [return: NotNullIfNotNull(nameof(defaultValue))]
        public static TEnum? ReadEnumOrDefault<TEnum>(this XmlAttributeCollection attributes, string name, TEnum? defaultValue = default) where TEnum : struct
        {
            XmlAttribute? child = attributes[name];
            return child == null ? defaultValue : child.InnerText.ToEnum<TEnum>(name);
        }

        public static TEnum? ReadEnumIfExists<TEnum>(this XmlAttributeCollection attributes, string name) where TEnum : struct
            => attributes[name]?.InnerText.ToEnum<TEnum>(name);

        #endregion

        #region SByte

        public static sbyte ReadSByte(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<sbyte>(name, provider);

        public static sbyte ReadSByteOrDefault(this XmlAttributeCollection attributes, string name, sbyte defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static sbyte? ReadSByteIfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<sbyte>(name, provider);

        #endregion

        #region Byte

        public static byte ReadByte(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<byte>(name, provider);

        public static byte ReadByteOrDefault(this XmlAttributeCollection attributes, string name, byte defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static byte? ReadByteIfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<byte>(name, provider);

        #endregion

        #region Int16

        public static short ReadInt16(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<short>(name, provider);

        public static short ReadInt16OrDefault(this XmlAttributeCollection attributes, string name, short defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static short? ReadInt16IfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<short>(name, provider);

        #endregion

        #region UInt16

        public static ushort ReadUInt16(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<ushort>(name, provider);

        public static ushort ReadUInt16OrDefault(this XmlAttributeCollection attributes, string name, ushort defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static ushort? ReadUInt16IfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<ushort>(name, provider);

        #endregion

        #region Int32

        public static int ReadInt32(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<int>(name, provider);

        public static int ReadInt32OrDefault(this XmlAttributeCollection attributes, string name, int defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static int? ReadInt32IfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<int>(name, provider);

        #endregion

        #region UInt32

        public static uint ReadUInt32(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<uint>(name, provider);

        public static uint ReadUInt32OrDefault(this XmlAttributeCollection attributes, string name, uint defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static uint? ReadUInt32IfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<uint>(name, provider);

        #endregion

        #region Int64

        public static long ReadInt64(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<long>(name, provider);

        public static long ReadInt64OrDefault(this XmlAttributeCollection attributes, string name, long defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static long? ReadInt64IfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<long>(name, provider);

        #endregion

        #region UInt64

        public static ulong ReadUInt64(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<ulong>(name, provider);

        public static ulong ReadUInt64OrDefault(this XmlAttributeCollection attributes, string name, ulong defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static ulong? ReadUInt64IfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<ulong>(name, provider);

        #endregion

        #region Int128

        public static Int128 ReadInt128(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<Int128>(name, provider);

        public static Int128 ReadInt128OrDefault(this XmlAttributeCollection attributes, string name, Int128 defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static Int128? ReadInt128IfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<Int128>(name, provider);

        #endregion

        #region UInt128

        public static UInt128 ReadUInt128(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<UInt128>(name, provider);

        public static UInt128 ReadUInt128OrDefault(this XmlAttributeCollection attributes, string name, UInt128 defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static UInt128? ReadUInt128IfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<UInt128>(name, provider);

        #endregion

        #region Half

        public static Half ReadHalf(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<Half>(name, provider);

        public static Half ReadHalfOrDefault(this XmlAttributeCollection attributes, string name, Half defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static Half? ReadHalfIfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<Half>(name, provider);

        #endregion

        #region Single

        public static float ReadSingle(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<float>(name, provider);

        public static float ReadSingleOrDefault(this XmlAttributeCollection attributes, string name, float defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static float? ReadSingleIfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<float>(name, provider);

        #endregion

        #region Double

        public static double ReadDouble(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<double>(name, provider);

        public static double ReadDoubleOrDefault(this XmlAttributeCollection attributes, string name, double defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static double? ReadDoubleIfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<double>(name, provider);

        #endregion

        #region Boolean

        public static bool ReadBoolean(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValue<bool>(name, provider);

        public static bool ReadBooleanOrDefault(this XmlAttributeCollection attributes, string name, bool defaultValue = default, IFormatProvider? provider = null)
            => attributes.ReadValueOrDefault(name, defaultValue, provider);

        public static bool? ReadBooleanIfExists(this XmlAttributeCollection attributes, string name, IFormatProvider? provider = null)
            => attributes.ReadValueIfExists<bool>(name, provider);

        #endregion

        #region String

        public static string ReadString(this XmlAttributeCollection attributes, string name)
            => attributes.GetAttributeOrThrow(name).InnerText;

        [return: NotNullIfNotNull(nameof(defaultValue))]
        public static string? ReadStringOrDefault(this XmlAttributeCollection attributes, string name, string? defaultValue = default)
        {
            XmlAttribute? child = attributes[name];
            return child?.InnerText ?? defaultValue;
        }

        public static string? ReadStringIfExists(this XmlAttributeCollection attributes, string name)
            => attributes[name]?.InnerText;

        public static string ReadStringOrThrowIfEmpty(this XmlAttributeCollection attributes, string name)
        {
            string value = attributes.ReadString(name);
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidDataException($"{name} cannot be null or empty.");
            }
            return value;
        }

        public static string ReadStringOrThrowIfWhiteSpace(this XmlAttributeCollection attributes, string name)
        {
            string value = attributes.ReadString(name);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidDataException($"{name} cannot be null, empty, or whitespace.");
            }
            return value;
        }

        #endregion
    }
}
