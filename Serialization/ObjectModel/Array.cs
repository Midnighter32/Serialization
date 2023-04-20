using Serialization.Core;
using Serialization.ObjectModel.Meta;
using System.Runtime.InteropServices;
using Encoder = Serialization.Core.Encoder;

namespace Serialization.ObjectModel
{
    internal class Array : Root
    {
        public Byte type = 0;
        public Int16 count = 0;
        public IList<byte> data;

        public Array ()
        {
            size += Marshal.SizeOf(type) + Marshal.SizeOf(count);
        }

        public static Array Create<T>(string name, IList<T> value) where T : struct, IConvertible
        {
            Array arr = new Array();
            arr.SetName(name);
            arr.wrapper = (Byte)Wrapper.Array;
            arr.type = (Byte)typeof(T).GetMeta();
            arr.count = (Int16)value.Count;
            arr.data = new byte[Marshal.SizeOf<T>() * arr.count];
            arr.size += value.Count * Marshal.SizeOf<T>();
            Int16 iterator = 0;
            Encoder.Encode(ref arr.data, ref iterator, value);

            return arr;
        }

        public static Array Create(string name, string value)
        {
            Array arr = new Array();
            arr.SetName(name);
            arr.wrapper = (Byte)Wrapper.String;
            arr.type = (Byte)typeof(string).GetMeta();
            arr.count = (Int16)value.Length;
            arr.data = new byte[value.Length];
            arr.size += value.Length;
            Int16 iterator = 0;
            Encoder.Encode(ref arr.data, ref iterator, value);

            return arr;
        }

        public override void Pack(ref IList<byte> buffer, ref Int16 iterator)
        {
            Encoder.Encode(ref buffer, ref iterator, wrapper);
            Encoder.Encode(ref buffer, ref iterator, nameLength);
            Encoder.Encode(ref buffer, ref iterator, name);
            Encoder.Encode(ref buffer, ref iterator, type);
            Encoder.Encode(ref buffer, ref iterator, count);
            Encoder.Encode(ref buffer, ref iterator, data);
            Encoder.Encode(ref buffer, ref iterator, size);
        }

        public static Array Unpack(ref IList<Byte> buffer, ref Int16 iterator)
        {
            Array arr = new Array();

            arr.wrapper = Encoder.Decode<Byte>(ref buffer, ref iterator);
            arr.nameLength = Encoder.Decode<Int16>(ref buffer, ref iterator);
            arr.name = Encoder.Decode(ref buffer, ref iterator, arr.nameLength);
            arr.type = Encoder.Decode<Byte>(ref buffer, ref iterator);
            arr.count = Encoder.Decode<Int16>(ref buffer, ref iterator);
            arr.data = new Byte[Util.GetTypeSize((Meta.Type)arr.type) * arr.count];
            Encoder.Decode<Byte>(ref buffer, ref iterator, ref arr.data);
            arr.size = Encoder.Decode<Int32>(ref buffer, ref iterator);

            return arr;
        }
    }
}
