using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arvin.MathSugar
{
    public class MVector : MLine
    {
        public MVector(MPoint start, MPoint end) : base(start, end)
        {
        }
    }

    public interface IVector<T> :ILine<T>
        where T : IMPoint
    {
        T Direction { get; set; }
    }
}
