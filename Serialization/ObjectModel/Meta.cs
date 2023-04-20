using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization.ObjectModel.Meta
{
    enum Wrapper : Byte
    {
        Primitive = 1,
        Array,
        String,
        Complex
    }

    enum Type : Byte
    {
        I8 = 1,
        I16,
        I32,
        I64,

        Float,
        Double,

        Bool,
        String
    }
}
