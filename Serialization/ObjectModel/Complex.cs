using Serialization.ObjectModel.Meta;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Encoder = Serialization.Core.Encoder;

namespace Serialization.ObjectModel
{
    internal class Complex : Root
    {
        private Byte entitiesCount = 0;
        private IList<Root> entities = new List<Root>();

        public Complex(string name = "")
        {
            SetName(name);
            wrapper = (Byte)Wrapper.Complex;
            size += Marshal.SizeOf<Byte>();
        }

        public void AddEntity(Root r)
        {
            entities.Add(r); 
            entitiesCount += 1;

            size += r.GetSize();
        }

        public override void Pack(ref IList<byte> buffer, ref short iterator)
        {
            Encoder.Encode(ref buffer, ref iterator, wrapper);
            Encoder.Encode(ref buffer, ref iterator, nameLength);
            Encoder.Encode(ref buffer, ref iterator, name);

            Encoder.Encode(ref buffer, ref iterator, entitiesCount);
            foreach (var e in entities)
            {
                e.Pack(ref buffer, ref iterator);
            }

            Encoder.Encode(ref buffer, ref iterator, size);
        }

        public static Complex Unpack(ref IList<byte> buffer, ref short iterator)
        {
            Complex complex = new Complex();

            complex.wrapper = Encoder.Decode<Byte>(ref buffer, ref iterator);
            complex.nameLength = Encoder.Decode<Int16>(ref buffer, ref iterator);
            complex.name = Encoder.Decode(ref buffer, ref iterator, complex.nameLength);

            complex.entitiesCount = Encoder.Decode<Byte>(ref buffer, ref iterator);

            for (int i = 0; i < complex.entitiesCount; i++)
            {
                var wrapper = Encoder.Decode<Byte>(ref buffer, ref iterator);
                iterator -= 1;

                Root obj = default;

                switch ((Wrapper)wrapper)
                {
                    case Wrapper.Primitive:
                        obj = Primitive.Unpack(ref buffer, ref iterator);
                        break;
                    case Wrapper.Array:
                    case Wrapper.String:
                        obj = Array.Unpack(ref buffer, ref iterator);
                        break;
                    case Wrapper.Complex:
                        obj = Complex.Unpack(ref buffer, ref iterator);
                        break;
                    default:
                        Debug.Fail("Type out of range");
                        break;
                }

                complex.entities.Add(obj);
            }

            complex.size = Encoder.Decode<Int32>(ref buffer, ref iterator);

            return complex;
        }

        public Int16 GetEntitiesCount() { return entitiesCount; }
    
        public Primitive FindPrimitiveByName(string name)
        {
            foreach (var e in entities)
            {
                if (e.wrapper == (Byte)Wrapper.Primitive && e.GetName() == name)
                {
                    return (Primitive)e;
                }
            }

            return Primitive.Create("SYSTEM:empty", 0);
        }

        public Root FindByName(string name)
        {
            foreach (var e in entities)
            {
                if (e.GetName() == name)
                {
                    return e;
                }
            }

            return new Complex("SYSTEM:empty");
        }

        public Root ElementAt(int index)
        {
            return entities.ElementAt(index);
        }
    }
}
