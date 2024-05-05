using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace YabberExtended.Extensions.Value
{
    public static class ValueExtensions
    {
        #region Generic

        public static TValue ToValue<TValue>(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null) where TValue : IParsable<TValue>
        {
            if (!TValue.TryParse(value, provider, out TValue? result))
            {
                throw new ParseException($"{valueName} could not be parsed as {typeof(TValue).Name}: {value}");
            }

            return result;
        }

        public static TEnum ToEnum<TEnum>(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null) where TEnum : struct
        {
            if (!Enum.TryParse(value, out TEnum result))
            {
                throw new ParseException($"{valueName} could not be parsed as {typeof(TEnum).Name}: {value}");
            }

            return result;
        }

        public static bool TryParseValue<TValue>([NotNullWhen(true)] this string? value, IFormatProvider? provider, [MaybeNullWhen(false)] out TValue valueResult) where TValue : IParsable<TValue>
        {
            if (!TValue.TryParse(value, provider, out valueResult))
            {
                return false;
            }

            return true;
        }

        public static bool TryParseEnum<TEnum>([NotNullWhen(true)] this string? value, out TEnum valueResult) where TEnum : struct
        {
            if (!Enum.TryParse(value, out valueResult))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Non-Generic

        public static sbyte ToSByte(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<sbyte>(valueName, provider);

        public static byte ToByte(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<byte>(valueName, provider);

        public static short ToInt16(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<short>(valueName, provider);

        public static ushort ToUInt16(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<ushort>(valueName, provider);

        public static int ToInt32(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<int>(valueName, provider);

        public static uint ToUInt32(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<uint>(valueName, provider);

        public static long ToInt64(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<long>(valueName, provider);

        public static ulong ToUInt64(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<ulong>(valueName, provider);

        public static Int128 ToInt128(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<Int128>(valueName, provider);

        public static UInt128 ToUInt128(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<UInt128>(valueName, provider);

        public static Half ToHalf(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<Half>(valueName, provider);

        public static float ToSingle(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<float>(valueName, provider);

        public static double ToDouble(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<double>(valueName, provider);

        public static bool ToBoolean(this string? value, [CallerArgumentExpression(nameof(value))] string? valueName = null, IFormatProvider? provider = null)
            => value.ToValue<bool>(valueName, provider);

        #endregion
    }
}
