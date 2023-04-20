using Serialization.Core;
using Serialization.ObjectModel.Meta;
using System.Numerics;
using System.Runtime.InteropServices;

#pragma warning disable CS8618

namespace Serialization.ObjectModel
{
    internal class Primitive : Root
    {
        private Byte type = 0;
        private IList<Byte> data;

        private Primitive() 
        {
            size += Marshal.SizeOf(type);
        }

        public static Primitive Create<T>(string name, T value) where T : struct, IConvertible
        {
            Primitive p = new Primitive();
            p.SetName(name);
            p.wrapper = (Byte)Wrapper.Primitive;
            p.type = (Byte)typeof(T).GetMeta();
            p.data = new Byte[Marshal.SizeOf<T>()];
            p.size += (Int32)p.data.Count();
            Int16 iterator = 0;

            Encoder.Encode(ref p.data, ref iterator, value);

            return p;
        }

        public override void Pack(ref IList<Byte> buffer, ref Int16 iterator)
        {
            Encoder.Encode(ref buffer, ref iterator, wrapper);
            Encoder.Encode(ref buffer, ref iterator, nameLength);
            Encoder.Encode(ref buffer, ref iterator, name);
            Encoder.Encode(ref buffer, ref iterator, type);
            Encoder.Encode(ref buffer, ref iterator, data);
            Encoder.Encode(ref buffer, ref iterator, size);
        }

        public static Primitive Unpack(ref IList<Byte> buffer, ref Int16 iterator)
        {
            Primitive p = new Primitive();

            p.wrapper = Encoder.Decode<Byte>(ref buffer, ref iterator);
            p.nameLength = Encoder.Decode<Int16>(ref buffer, ref iterator);
            p.name = Encoder.Decode(ref buffer, ref iterator, p.nameLength);
            p.type = Encoder.Decode<Byte>(ref buffer, ref iterator);
            p.data = new Byte[Util.GetTypeSize((Meta.Type)p.type)];
            Encoder.Decode(ref buffer, ref iterator, ref p.data);
            p.size = Encoder.Decode<Int32>(ref buffer, ref iterator);

            return p;
        }

        public IList<Byte> GetData() { return data; }
        public ref IList<Byte> GetPtrData() { return ref data; }
    }
}
