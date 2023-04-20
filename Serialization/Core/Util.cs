using Newtonsoft.Json.Linq;
using Serialization.ObjectModel;
using Serialization.ObjectModel.Meta;
using System.Collections;
using System.Diagnostics;
using Array = Serialization.ObjectModel.Array;
using Type = Serialization.ObjectModel.Meta.Type;

namespace Serialization.Core
{
    internal class Util
    {
        public static bool IsLittleEndian(Byte a)
        {
            Byte[] b = new Byte[] { a };
            BitArray vector = new BitArray(b);

            return vector[0];
        }

        public static Byte GetTypeSize(Type type)
        {
            switch (type)
            {
                case Type.Bool: return sizeof(Boolean);
                case Type.I8: return sizeof(Byte);
                case Type.I16: return sizeof(Int16);
                case Type.I32: return sizeof(Int32);
                case Type.I64: return sizeof(Int64);
                case Type.Float: return sizeof(Single);
                case Type.Double: return sizeof(Double);
                case Type.String: return sizeof(Byte);
                default: return 0;
            }
        }

        public static void RetriveNSave(Root r)
        {
            Int16 iterator = 0;

            IList<Byte> buffer = new Byte[r.GetSize()];
            string name = r.GetName() + ".psf";

            r.Pack(ref buffer, ref iterator);

            Console.Write(r.GetName() + " Dump: "); Program.Dump(buffer);

            Save(name, buffer);
        }

        public static void LoadNDump(string file)
        {
            var objectFromFile = Util.Load(file);

            JObject obj = new JObject();

            Int16 iterator = 0;
            var root = Deserialize(ref objectFromFile, ref iterator);
            Dump(ref root, ref obj);

            Console.WriteLine(obj.ToString(Newtonsoft.Json.Formatting.Indented));
        }

        public static Root Deserialize(ref IList<Byte> buffer, ref Int16 iterator)
        {
            var wrapper = Encoder.Decode<Byte>(ref buffer, ref iterator);
            iterator -= 1;

            Root root = default;

            switch ((Wrapper)wrapper)
            {
                case Wrapper.Primitive:
                    root = Primitive.Unpack(ref buffer, ref iterator);
                    break;
                case Wrapper.Array:
                case Wrapper.String:
                    root = Array.Unpack(ref buffer, ref iterator);
                    break;
                case Wrapper.Complex:
                    root = Complex.Unpack(ref buffer, ref iterator);
                    break;
                default:
                    Debug.Fail("Type out of range");
                    break;
            }

            return root;
        }

        public static void Dump(ref Root root, ref JObject obj)
        {
            Int16 it = 0;
            switch ((Wrapper)root.wrapper)
            {
                case Wrapper.Primitive:
                    var prim = (Primitive)root;
                    var data = prim.GetData();
                    obj[root.GetName()] = Encoder.Decode<Int32>(ref data, ref it);
                    break;
                case Wrapper.Array:
                    var arr = (Array)root;
                    IList<Int32> result = new Int32[arr.count];
                    Encoder.Decode(ref arr.data, ref it, ref result);
                    obj[root.GetName()] = new JArray(result.ToArray());
                    break;
                case Wrapper.String:
                    var str = (Array)root;
                    obj[root.GetName()] = Encoder.Decode(ref str.data, ref it, str.count);
                    break;
                case Wrapper.Complex:
                    var coml = (Complex)root;
                    JObject child = new JObject();
                    for (int i = 0; i < coml.GetEntitiesCount(); i++)
                    {
                        var el = coml.ElementAt(i);

                        Dump(ref el, ref child);
                    }
                    obj[root.GetName()] = child;
                    break;
                default:
                    Debug.Fail("Type out of range");
                    break;
            }
        }

        private static void Save(string file, IList<Byte> data)
        {
            using var stream = File.OpenWrite(file);

            stream.Write(data.ToArray(), 0, data.Count());

            stream.Close();
        }

        public static IList<Byte> Load(string file)
        {
            if (!File.Exists(file))
            {
                Debug.Fail("File should exist");
            }

            using var stream = File.OpenRead(file);

            Byte[] data = new Byte[stream.Length];
            stream.Read(data, 0, data.Length);

            stream.Close();

            return data;
        }
    }

    internal static class TypeExtension
    {
        public static Type GetMeta(this System.Type type)
        {
            switch (type.FullName)
            {
                case "System.Boolean": return Type.Bool;
                case "System.Byte": return Type.I8;
                case "System.Int16":
                case "System.UInt16": return Type.I16;
                case "System.Int32":
                case "System.UInt32": return Type.I32;
                case "System.Int64":
                case "System.UInt64": return Type.I64;
                case "System.Single": return Type.Float;
                case "System.Double": return Type.Double;
                case "System.String": return Type.String;
                default: return 0;
            }
        }
    }
}
