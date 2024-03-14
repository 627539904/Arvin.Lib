using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arvin.MathSugar.Models
{
    /// <summary>
    /// 直线/线段
    /// </summary>
    public class MLine:ILine<MPoint>
    {
        public MPoint Start { get; set; }
        public MPoint End { get; set; }
        public double Length { get; set; }

        public MLine(MPoint start, MPoint end)
        {
            this.Start = start;
            this.End = end;
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
