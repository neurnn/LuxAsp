using System;
using System.IO;
using System.Linq;
using System.Text;
using LuxAsp.Sessions;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace LuxAsp
{
    public static class ILuxSessionExtensions
    {
        /// <summary>
        /// Get BSON encoded object from the Session.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static TObject GetBSON<TObject>(this ILuxSession This, string Key, Func<TObject> Default = null)
        {
            var Bytes = This.Get(Key);
            if (Bytes != null)
            {
                using var Stream = new MemoryStream(Bytes);
                using(var Reader = new BsonDataReader(Stream))
                {
                    var Serializer = new JsonSerializer();
                    try { return Serializer.Deserialize<TObject>(Reader); }
                    catch { }
                }
            }

            return Default != null ? Default() : default;
        }

        /// <summary>
        /// Set BSON encoded object into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ILuxSession SetBSON(this ILuxSession This, string Key, object Value)
        {
            using var Stream = new MemoryStream();
            using(var Writer = new BsonDataWriter(Stream))
            {
                var Serializer = new JsonSerializer();
                try { Serializer.Serialize(Writer, Value); }
                catch { }
            }

            return This.Set(Key, Stream.ToArray());
        }

        /// <summary>
        /// Get Date Time from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(this ILuxSession This, string Key, Func<DateTime> Default = null)
        {
            var Value = GetInt64(This, Key, -1);
            if (Value < 0)
            {
                if (Default is null)
                    return DateTime.MinValue;

                return Default();
            }

            return new DateTime(Value, DateTimeKind.Utc);
        }

        /// <summary>
        /// Set Date Time into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ILuxSession SetDateTime(this ILuxSession This, string Key, DateTime Value)
        {
            if (Value.Kind != DateTimeKind.Utc)
                Value = Value.ToUniversalTime();

            return This.SetInt64(Key, Value.Ticks);
        }

        /// <summary>
        /// Get TimeSpan from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpan(this ILuxSession This, string Key, TimeSpan Default = default)
        {
            var Value = GetInt64(This, Key, -1);
            return Value < 0 ? Default : new TimeSpan(Value);
        }

        /// <summary>
        /// Set TimeSpan into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ILuxSession SetTimeSpan(this ILuxSession This, string Key, TimeSpan Value)
            => This.SetInt64(Key, Value.Ticks);

        /// <summary>
        /// Get GUID from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static Guid GetGuid(this ILuxSession This, string Key, Guid Default = default)
        {
            var Bytes = This.Get(Key);
            if (Bytes is null || Bytes.Length < 16)
                return Default;

            return new Guid(Bytes);
        }

        /// <summary>
        /// Set GUID into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ILuxSession SetGuid(this ILuxSession This, string Key, Guid Value)
            => This.Set(Key, Value.ToByteArray());

        /// <summary>
        /// Get Char from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static char GetChar(this ILuxSession This, string Key, char Default = '\0')
        {
            var Bytes = This.Get(Key);
            if (Bytes is null || Bytes.Length <= 0)
                return Default;

            try { return Encoding.UTF8.GetChars(Bytes).First(); }
            catch { }

            return Default;
        }

        /// <summary>
        /// Set Char into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ILuxSession SetChar(this ILuxSession This, string Key, char Value) 
            => This.Set(Key, Encoding.UTF8.GetBytes(new char[] { Value }));

        /// <summary>
        /// Get Byte from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static byte GetByte(this ILuxSession This, string Key, byte Default = 0)
        {
            var Bytes = This.Get(Key);
            if (Bytes is null || Bytes.Length <= 0)
                return Default;

            return Bytes.First();
        }

        /// <summary>
        /// Set Byte into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ILuxSession SetByte(this ILuxSession This, string Key, byte Value)
            => This.Set(Key, new byte[] { Value });

        /// <summary>
        /// Get String from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static string GetString(this ILuxSession This, string Key, string Default = null)
        {
            var Bytes = This.Get(Key);
            if (Bytes is null || Bytes.Length <= 0)
                return Default;

            try { return Encoding.UTF8.GetString(Bytes); }
            catch { }

            return Default;
        }

        /// <summary>
        /// Set String into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ILuxSession SetString(this ILuxSession This, string Key, string Value) 
            => This.Set(Key, Encoding.UTF8.GetBytes(Value));

        /// <summary>
        /// Make the Int64 to be in range.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        private static long Range(long Value, long Min, long Max)
            => Math.Max(Math.Min(Value, Max), Min);

        /// <summary>
        /// Get Int16 from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static short GetInt16(this ILuxSession This, string Key, short Default = 0)
            => (short)Range(GetInt64(This, Key, Default), short.MinValue, short.MaxValue);

        /// <summary>
        /// Get Int32 from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static int GetInt32(this ILuxSession This, string Key, int Default = 0)
             => (short)Range(GetInt32(This, Key, Default), int.MinValue, int.MaxValue);

        /// <summary>
        /// Get Int64 from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static long GetInt64(this ILuxSession This, string Key, long Default = 0)
        {
            var Bytes = This.Get(Key);
            if (Bytes is null)
                return Default;

            long Result = 0;
            for (int i = 0; i < Math.Max(Bytes.Length, sizeof(long)); ++i)
                Result |= (((long)Bytes[i]) << (8 * i));

            return Result;
        }

        /// <summary>
        /// Get UInt16 from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static ushort GetUInt16(this ILuxSession This, string Key, ushort Default = 0)
            => (ushort)Math.Min(GetUInt64(This, Key, Default), ushort.MaxValue);

        /// <summary>
        /// Get UInt32 from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static uint GetUInt32(this ILuxSession This, string Key, uint Default = 0)
             => (uint)Math.Min(GetUInt64(This, Key, Default), uint.MaxValue);

        /// <summary>
        /// Get Int64 from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static ulong GetUInt64(this ILuxSession This, string Key, ulong Default = 0)
        {
            var Bytes = This.Get(Key);
            if (Bytes is null)
                return Default;

            ulong Result = 0;
            for (int i = 0; i < Math.Max(Bytes.Length, sizeof(long)); ++i)
                Result |= (((ulong)Bytes[i]) << (8 * i));

            return Result;
        }

        /// <summary>
        /// Set Int16 into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ILuxSession SetInt16(this ILuxSession This, string Key, short Value)
            => SetIntXX(This, Key, Value, sizeof(short));

        /// <summary>
        /// Set Int32 into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ILuxSession SetInt32(this ILuxSession This, string Key, int Value)
            => SetIntXX(This, Key, Value, sizeof(int));

        /// <summary>
        /// Set Int64 into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ILuxSession SetInt64(this ILuxSession This, string Key, long Value)
            => SetIntXX(This, Key, Value, sizeof(long));

        /// <summary>
        /// Set UInt16 into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ILuxSession SetUInt16(this ILuxSession This, string Key, ushort Value)
            => SetUIntXX(This, Key, Value, sizeof(short));

        /// <summary>
        /// Set UInt32 into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ILuxSession SetUInt32(this ILuxSession This, string Key, uint Value)
            => SetUIntXX(This, Key, Value, sizeof(int));

        /// <summary>
        /// Set UInt64 into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static ILuxSession SetUInt64(this ILuxSession This, string Key, ulong Value)
            => SetUIntXX(This, Key, Value, sizeof(long));

        /// <summary>
        /// Set X bit Integer into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        private static ILuxSession SetIntXX(this ILuxSession This, string Key, long Value, int Length = sizeof(long))
        {
            var Bytes = new byte[Length];
            for (int i = 0; i < Bytes.Length; ++i)
                Bytes[i] = (byte)((Value >> (8 * i)) & 0xff);

            This.Set(Key, Bytes);
            return This;
        }

        /// <summary>
        /// Set X bit Integer into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        private static ILuxSession SetUIntXX(this ILuxSession This, string Key, ulong Value, int Length = sizeof(long))
        {
            var Bytes = new byte[Length];
            for (int i = 0; i < Bytes.Length; ++i)
                Bytes[i] = (byte)((Value >> (8 * i)) & 0xff);

            This.Set(Key, Bytes);
            return This;
        }

        /// <summary>
        /// Get Single from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static float GetSingle(this ILuxSession This, string Key, float Default = 0)
        {
            var Bytes = This.Get(Key);
            if (Bytes is null || Bytes.Length < sizeof(float))
                return Default;

            if (Bytes.Length != sizeof(float))
                Array.Resize(ref Bytes, sizeof(float));

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(Bytes);

            return BitConverter.ToSingle(Bytes);
        }

        /// <summary>
        /// Set Single into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static ILuxSession SetSingle(this ILuxSession This, string Key, float Value)
        {
            var Bytes = BitConverter.GetBytes(Value);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(Bytes);

            return This.Set(Key, Bytes);
        }

        /// <summary>
        /// Get Double from the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static double GetDouble(this ILuxSession This, string Key, double Default = 0)
        {
            var Bytes = This.Get(Key);
            if (Bytes is null || Bytes.Length < sizeof(double))
                return Default;

            if (Bytes.Length != sizeof(double))
                Array.Resize(ref Bytes, sizeof(double));

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(Bytes);

            return BitConverter.ToSingle(Bytes);
        }

        /// <summary>
        /// Set Double into the Session.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static ILuxSession SetDouble(this ILuxSession This, string Key, double Value)
        {
            var Bytes = BitConverter.GetBytes(Value);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(Bytes);

            return This.Set(Key, Bytes);
        }
    }
}
