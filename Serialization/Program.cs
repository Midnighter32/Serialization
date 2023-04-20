using System;
using System.Diagnostics;
using System.Reflection;
using Serialization.Core;
using Serialization.ObjectModel;
using Array = Serialization.ObjectModel.Array;

namespace TestFrame
{
    class Test
    {
        internal static void TestPack()
        {
            Complex obj = new Complex("obj");

            var foo = Primitive.Create("foo", 123);
            var bar = Array.Create("bar", "something");
            var baz = Array.Create("baz", new int[] { 1, 2, 3 });

            obj.AddEntity(foo);
            obj.AddEntity(bar);
            obj.AddEntity(baz);

            Util.RetriveNSave(foo);
            Util.RetriveNSave(bar);
            Util.RetriveNSave(baz);

            Util.RetriveNSave(obj);
        }

        internal static void TestUnpack()
        {
            IList<Byte> foo = Util.Load("foo.psf");

            Int16 it = 0;
            Primitive foo2 = Primitive.Unpack(ref foo, ref it);

            var data = foo2.GetData();

            Int16 it2 = 0;
            Int32 primitive = Encoder.Decode<Int32>(ref data, ref it2);

            Console.WriteLine("Primitive Value: " + primitive);

            IList<Byte> bar = Util.Load("bar.psf");

            it = 0;
            Array bar2 = Array.Unpack(ref bar, ref it);

            var data2 = bar2.data;

            it2 = 0;
            string str = Encoder.Decode(ref data2, ref it2, bar2.count);

            Console.WriteLine("String Value: " + str);

            IList<Byte> baz = Util.Load("baz.psf");

            it = 0;
            Array baz2 = Array.Unpack(ref baz, ref it);

            var data3 = baz2.data;

            it2 = 0;
            IList<Int32> arr = new Int32[baz2.count];
            Encoder.Decode(ref data3, ref it2, ref arr);

            Console.WriteLine("Array Value: " + string.Join(", ", arr));

            IList<Byte> obj = Util.Load("obj.psf");

            it = 0;
            Complex obj2 = Complex.Unpack(ref obj, ref it);

            var data4 = obj2.FindPrimitiveByName("foo").GetData();

            it2 = 0;
            Int32 obj_primitive = Encoder.Decode<Int32>(ref data4, ref it2);

            var array = obj2.FindByName("bar") as Array;
            IList<Byte> data5 = (IList<byte>)(array?.data ?? Enumerable.Empty<Byte>());

            it2 = 0;
            string obj_str = Encoder.Decode(ref data5, ref it2, array?.count ?? 0);

            var array2 = obj2.FindByName("baz") as Array;
            IList<Byte> data6 = (IList<byte>)(array2?.data ?? Enumerable.Empty<Byte>());

            it2 = 0;
            IList<Int32> obj_array = new Int32[array2?.count ?? 0];
            Encoder.Decode(ref data6, ref it2, ref obj_array);

            Console.WriteLine("Object.Primitive Value: " + obj_primitive);
            Console.WriteLine("Object.String Value: " + obj_str);
            Console.WriteLine("Object.Array Value: " + string.Join(", ", obj_array));
        }
    }
}

namespace Serialization
{
    internal class Program
    {
        public static void Dump(IEnumerable<Byte> buffer)
        {
            var last = -1;
            buffer.Select((value, index) => (value, index))
                .LastOrDefault(x => { if (x.value > 0) last = x.index; return x.value > 0; });

            var msg = string.Join(", ", buffer.Take(last + 1).Select(x => "0x" + x.ToString("X2")));

            Console.WriteLine(msg);
        }

        static void Main(string[] args)
        {
            Debug.Assert(Util.IsLittleEndian(5), "Supports only Little Endian");

            TestFrame.Test.TestPack();
            TestFrame.Test.TestUnpack();
        }
    }
}