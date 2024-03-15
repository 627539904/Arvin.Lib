using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arvin.MathSugar
{
    /// <summary>
    /// 圆弧（从起点到终点逆时针绘制）
    /// </summary>
    public class MArc:MCurve
    {
        /// <summary>
        /// 圆心
        /// </summary>
        public MPoint Center { get; set; }
        /// <summary>
        /// 半径
        /// </summary>
        public double Radius {  get; set; }

        public override double GetLength()
        {
            return base.GetLength();
        }

        public override MPoint GetMid()
        {
            return base.GetMid();
        }
    }
}
