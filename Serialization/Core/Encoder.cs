using System.Drawing;
using System.Runtime.InteropServices;

namespace Serialization.Core
{
    internal class Encoder
    {
        public static void Encode<T>(ref IList<Byte> buffer, ref Int16 iterator, T value) where T : struct, IConvertible
        {
            Byte[] Bytes = new Byte[Marshal.SizeOf(value)];
            GCHandle handle = default(GCHandle);

            try
            {
                handle = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
                Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }

            for (Int32 i = 0; i < Bytes.Length; i++)
            {
                buffer[iterator++] = Bytes[i];
            }
        }

        public static void Encode(ref IList<Byte> buffer, ref Int16 iterator, string value)
        {
            for (UInt16 i = 0; i < value.Length; i++)
            {
                Encode(ref buffer, ref iterator, value[i]);
            }
        }

        public static void Encode<T>(ref IList<Byte> buffer, ref Int16 iterator, IList<T> value) where T : struct, IConvertible
        {
            for (UInt16 i = 0; i < value.Count(); i++)
            {
                Encode(ref buffer, ref iterator, value[i]);
            }
        }


        public static T Decode<T>(ref IList<Byte> buffer, ref Int16 iterator) where T : struct, IConvertible
        {
            Int16 size = (Int16)Marshal.SizeOf(typeof(T));
            GCHandle handle = default(GCHandle);

            var Bytes = buffer.Take(new Range(iterator, iterator + size)).ToArray();

            T value = default;

            try
            {
                handle = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
                value = (T?)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T)) ?? default;
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }

                iterator += size;
            }

            return value;
        }

        public static string Decode(ref IList<Byte> buffer, ref Int16 iterator, Int16 length)
        {
            string value = string.Empty;

            for (UInt16 i = 0; i < length; i++)
            {
                value += Decode<Char>(ref buffer, ref iterator);
            }

            return value;
        }

        public static void Decode<T>(ref IList<Byte> buffer, ref Int16 iterator, ref IList<T> dest) where T : struct, IConvertible
        {
            for (UInt16 i = 0; i < dest.Count(); i++)
            {
                dest[i] = Decode<T>(ref buffer, ref iterator);
            }
        }
    }
}
