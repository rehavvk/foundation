// SOURCE: https://gist.github.com/RiskyWilhelm/7c7d3dccc212476d065a86641b7ae419

using System;
using System.Collections.Generic;
using System.Linq;
using Rehawk.Foundation.Misc;

namespace Rehawk.Foundation.Extensions
{
    public static class EnumExtensions
    {
	    public static IEnumerable<EnumType> GetFlags<EnumType>(this EnumType value)
		    where EnumType : Enum
	    {
		    EnumUtility.ValidateFlagEnum<EnumType>();
		    foreach (EnumType flag in Enum.GetValues(typeof(EnumType)).Cast<EnumType>())
		    {
			    if (value.HasFlag(flag))
				    yield return flag;
		    }
	    }
	    
        /// <summary> Checks if enum 'a' contains any of the value(s) from 'b' </summary>
		public static bool HasAny<EnumType>(this EnumType a, EnumType b)
			where EnumType : Enum
		{
			// Check for the supported enum types
			return a.GetTypeCode() switch
			{
				// byte
				TypeCode.Byte => HasAny(a, (byte)(object)b),
				// sbyte
				TypeCode.SByte => HasAny(a, (sbyte)(object)b),
				// short
				TypeCode.Int16 => HasAny(a, (short)(object)b),
				// ushort
				TypeCode.UInt16 => HasAny(a, (ushort)(object)b),
				// int
				TypeCode.Int32 => HasAny(a, (int)(object)b),
				// uint
				TypeCode.UInt32 => HasAny(a, (uint)(object)b),
				// long
				TypeCode.Int64 => HasAny(a, (long)(object)b),
				// ulong
				TypeCode.UInt64 => HasAny(a, (ulong)(object)b),
				_ => throw new NotSupportedException("Unknown Error. This shouldn't be happened!"),
			};
		}

		public static bool HasAny<EnumType>(this EnumType a, byte b)
			where EnumType : Enum
		{
			EnumUtility.ValidateFlagEnum<EnumType>();
			EnumUtility.ValidateUnderlyingType<EnumType, byte>();
			return ((byte)(object)a & b) != 0;
		}

		/// <summary> Checks if enum 'a' contains any of the value(s) from 'b' </summary>
		public static bool HasAny<EnumType>(this EnumType a, sbyte b)
			where EnumType : Enum
		{
			EnumUtility.ValidateFlagEnum<EnumType>();
			EnumUtility.ValidateUnderlyingType<EnumType, sbyte>();
			return ((sbyte)(object)a & b) != 0;
		}

		/// <summary> Checks if enum 'a' contains any of the value(s) from 'b' </summary>
		public static bool HasAny<EnumType>(this EnumType a, short b)
			where EnumType : Enum
		{
			EnumUtility.ValidateFlagEnum<EnumType>();
			EnumUtility.ValidateUnderlyingType<EnumType, short>();
			return ((short)(object)a & b) != 0;
		}

		/// <summary> Checks if enum 'a' contains any of the value(s) from 'b' </summary>
		public static bool HasAny<EnumType>(this EnumType a, ushort b)
			where EnumType : Enum
		{
			EnumUtility.ValidateFlagEnum<EnumType>();
			EnumUtility.ValidateUnderlyingType<EnumType, ushort>();
			return ((ushort)(object)a & b) != 0;
		}

		/// <summary> Checks if enum 'a' contains any of the value(s) from 'b' </summary>
		public static bool HasAny<EnumType>(this EnumType a, int b)
			where EnumType : Enum
		{
			EnumUtility.ValidateFlagEnum<EnumType>();
			EnumUtility.ValidateUnderlyingType<EnumType, int>();
			return ((int)(object)a & b) != 0;
		}

		/// <summary> Checks if enum 'a' contains any of the value(s) from 'b' </summary>
		public static bool HasAny<EnumType>(this EnumType a, uint b)
			where EnumType : Enum
		{
			EnumUtility.ValidateFlagEnum<EnumType>();
			EnumUtility.ValidateUnderlyingType<EnumType, uint>();
			return ((uint)(object)a & b) != 0;
		}

		/// <summary> Checks if enum 'a' contains any of the value(s) from 'b' </summary>
		public static bool HasAny<EnumType>(this EnumType a, long b)
			where EnumType : Enum
		{
			EnumUtility.ValidateFlagEnum<EnumType>();
			EnumUtility.ValidateUnderlyingType<EnumType, long>();
			return ((long)(object)a & b) != 0;
		}

		/// <summary> Checks if enum 'a' contains any of the value(s) from 'b' </summary>
		public static bool HasAny<EnumType>(this EnumType a, ulong b)
			where EnumType : Enum
		{
			EnumUtility.ValidateFlagEnum<EnumType>();
			EnumUtility.ValidateUnderlyingType<EnumType, ulong>();
			return ((ulong)(object)a & b) != 0;
		}
    }
}