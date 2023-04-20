using Serialization.ObjectModel;
using System.Collections;
using System.Diagnostics;
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

            Program.Dump(buffer);

            Save(name, buffer);
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
