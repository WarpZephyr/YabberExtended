using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using YabberExtended.Extensions;
using YabberExtended.Extensions.Value;

namespace YabberExtended.Parse
{
    /// <summary>
    /// A dictionary that can hold many different named values as strings and provides methods to parse them.
    /// </summary>
    public class ValueDictionary : Dictionary<string, string>
    {
        /// <summary>
        /// Provides an error message and easier access for <see cref="DictionaryExtensions.GetValueOrThrow{TKey, TValue}(Dictionary{TKey, TValue}, TKey, string?)"/>
        /// </summary>
        /// <param name="key">The key to get the value of.</param>
        /// <returns>The value of the given key.</returns>
        private string GetValueOrThrow(string key)
            => this.GetValueOrThrow(key, $"Could not find {key} in the {nameof(ValueDictionary)}");

        #region Generic

        public TValue ReadValue<TValue>(string valueName, IFormatProvider? provider = null) where TValue : IParsable<TValue>
            => GetValueOrThrow(valueName).ToValue<TValue>(valueName, provider);

        [return: NotNullIfNotNull(nameof(defaultValue))]
        public TValue? ReadValueOrDefault<TValue>(string valueName, TValue? defaultValue = default, IFormatProvider? provider = null) where TValue : IParsable<TValue>
        {
            if (!TryGetValue(valueName, out string? str))
            {
                return defaultValue;
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                return defaultValue;
            }

            return str.ToValue<TValue>(valueName, provider);
        }

        public TValue? ReadValueIfExists<TValue>(string valueName, IFormatProvider? provider = null) where TValue : struct, IParsable<TValue>
        {
            if (!TryGetValue(valueName, out string? str))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            return str.ToValue<TValue>(valueName, provider);
        }

        public bool TryReadValue<TValue>(string valueName, IFormatProvider? provider, [MaybeNullWhen(false)] out TValue value) where TValue : IParsable<TValue>
        {
            if (!TryGetValue(valueName, out string? str))
            {
                value = default;
                return false;
            }

            value = str.ToValue<TValue>(valueName, provider);
            return true;
        }

        public TEnum ReadEnum<TEnum>(string valueName) where TEnum : struct
            => GetValueOrThrow(valueName).ToEnum<TEnum>(valueName);

        public TEnum ReadEnumOrDefault<TEnum>(string valueName, TEnum defaultValue = default) where TEnum : struct
        {
            if (!TryGetValue(valueName, out string? str))
            {
                return defaultValue;
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                return defaultValue;
            }

            return str.ToEnum<TEnum>(valueName);
        }

        public TEnum? ReadEnumIfExists<TEnum>(string valueName) where TEnum : struct
        {
            if (!TryGetValue(valueName, out string? str))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            return str.ToEnum<TEnum>(valueName);
        }

        public bool TryReadEnum<TEnum>(string valueName, out TEnum value) where TEnum : struct
        {
            if (!TryGetValue(valueName, out string? str))
            {
                value = default;
                return false;
            }

            value = str.ToEnum<TEnum>(valueName);
            return true;
        }

        #endregion

        #region SByte

        public sbyte ReadSByte(string valueName, IFormatProvider? provider = null)
            => ReadValue<sbyte>(valueName, provider);

        public sbyte ReadSByteOrDefault(string valueName, sbyte defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public sbyte? ReadSByteIfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<sbyte>(valueName, provider);

        #endregion

        #region Byte

        public byte ReadByte(string valueName, IFormatProvider? provider = null)
            => ReadValue<byte>(valueName, provider);

        public byte ReadByteOrDefault(string valueName, byte defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public byte? ReadByteIfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<byte>(valueName, provider);

        #endregion

        #region Int16

        public short ReadInt16(string valueName, IFormatProvider? provider = null)
            => ReadValue<short>(valueName, provider);

        public short ReadInt16OrDefault(string valueName, short defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public short? ReadInt16IfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<short>(valueName, provider);

        #endregion

        #region UInt16

        public ushort ReadUInt16(string valueName, IFormatProvider? provider = null)
            => ReadValue<ushort>(valueName, provider);

        public ushort ReadUInt16OrDefault(string valueName, ushort defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public ushort? ReadUInt16IfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<ushort>(valueName, provider);

        #endregion

        #region Int32

        public int ReadInt32(string valueName, IFormatProvider? provider = null)
            => ReadValue<int>(valueName, provider);

        public int ReadInt32OrDefault(string valueName, int defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public int? ReadInt32IfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<int>(valueName, provider);

        #endregion

        #region UInt32

        public uint ReadUInt32(string valueName, IFormatProvider? provider = null)
            => ReadValue<uint>(valueName, provider);

        public uint ReadUInt32OrDefault(string valueName, uint defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public uint? ReadUInt32IfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<uint>(valueName, provider);

        #endregion

        #region Int64

        public long ReadInt64(string valueName, IFormatProvider? provider = null)
            => ReadValue<long>(valueName, provider);

        public long ReadInt64OrDefault(string valueName, long defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public long? ReadInt64IfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<long>(valueName, provider);

        #endregion

        #region UInt64

        public ulong ReadUInt64(string valueName, IFormatProvider? provider = null)
            => ReadValue<ulong>(valueName, provider);

        public ulong ReadUInt64OrDefault(string valueName, ulong defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public ulong? ReadUInt64IfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<ulong>(valueName, provider);

        #endregion

        #region Int128

        public Int128 ReadInt128(string valueName, IFormatProvider? provider = null)
            => ReadValue<Int128>(valueName, provider);

        public Int128 ReadInt128OrDefault(string valueName, Int128 defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public Int128? ReadInt128IfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<Int128>(valueName, provider);

        #endregion

        #region UInt128

        public UInt128 ReadUInt128(string valueName, IFormatProvider? provider = null)
            => ReadValue<UInt128>(valueName, provider);

        public UInt128 ReadUInt128OrDefault(string valueName, UInt128 defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public UInt128? ReadUInt128IfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<UInt128>(valueName, provider);

        #endregion

        #region Half

        public Half ReadHalf(string valueName, IFormatProvider? provider = null)
            => ReadValue<Half>(valueName, provider);

        public Half ReadHalfOrDefault(string valueName, Half defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public Half? ReadHalfIfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<Half>(valueName, provider);

        #endregion

        #region Single

        public float ReadSingle(string valueName, IFormatProvider? provider = null)
            => ReadValue<float>(valueName, provider);

        public float ReadSingleOrDefault(string valueName, float defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public float? ReadSingleIfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<float>(valueName, provider);

        #endregion

        #region Double

        public double ReadDouble(string valueName, IFormatProvider? provider = null)
            => ReadValue<double>(valueName, provider);

        public double ReadDoubleOrDefault(string valueName, double defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public double? ReadDoubleIfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<double>(valueName, provider);

        #endregion

        #region Boolean

        public bool ReadBoolean(string valueName, IFormatProvider? provider = null)
            => ReadValue<bool>(valueName, provider);

        public bool ReadBooleanOrDefault(string valueName, bool defaultValue = default, IFormatProvider? provider = null)
            => ReadValueOrDefault(valueName, defaultValue, provider);

        public bool? ReadBooleanIfExists(string valueName, IFormatProvider? provider = null)
            => ReadValueIfExists<bool>(valueName, provider);

        #endregion

        #region String

        public string ReadString(string valueName)
            => GetValueOrThrow(valueName);

        [return: NotNullIfNotNull(nameof(defaultValue))]
        public string? ReadStringOrDefault(string valueName, string? defaultValue = default)
        {
            if (!TryGetValue(valueName, out string? value))
            {
                return defaultValue;
            }

            return value;
        }

        public string? ReadStringIfExists(string valueName)
        {
            if (!TryGetValue(valueName, out string? value))
            {
                return null;
            }

            return value;
        }

        #endregion
    }
}
