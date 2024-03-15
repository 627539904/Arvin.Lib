using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arvin.MathSugar
{
    /// <summary>
    /// 直线、弧线基本类
    /// </summary>
    public class MCurve : IVector<MPoint>
    {
        public MPoint Start { get; set; }
        public MPoint End { get; set; }
        public double Length { get; set; }
        public MPoint Mid { get; set; }
        /// <summary>
        /// 起点到终点的方向，单位向量
        /// </summary>
        public MPoint Direction { get; set; }

        public MCurve()
        {
            this.Direction=GetDirection();
        }

        public virtual double GetLength()
        {
            return this.Start.Distance(this.End);
        }

        public virtual MPoint GetMid()
        {
            return this.Start.Mid(this.End);
        }
        /// <summary>
        /// 起点到终点的方向
        /// </summary>
        /// <returns></returns>
        public virtual MPoint GetDirection()
        {
            return Start.Direction(this.End).ToUnitNormalize();
        }
    }
}
