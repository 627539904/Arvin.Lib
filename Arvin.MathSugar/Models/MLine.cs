using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arvin.MathSugar
{
    /// <summary>
    /// 直线/线段
    /// </summary>
    public class MLine:MCurve
    {
        public MLine(MPoint start, MPoint end)
        {
            this.Start = start;
            this.End = end;
            this.Length = GetLength();
            this.Mid= GetMid();
        }
    }

    /// <summary>
    /// 直线/线段-数学特征
    /// </summary>
    /// <typeparam name="TPoint"></typeparam>
    public interface ILine<TPoint>
        where TPoint : IMPoint
    {
        TPoint Start { get; set; }
        TPoint End { get; set; }
        double Length { get; set; }
    }
}
