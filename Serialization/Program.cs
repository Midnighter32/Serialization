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
            Util.LoadNDump("foo.psf");
            Util.LoadNDump("bar.psf");
            Util.LoadNDump("baz.psf");

            Util.LoadNDump("obj.psf");
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