using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arvin.MathSugar.Models
{
    /// <summary>
    /// 点-几何基础特征
    /// </summary>
    public class MPoint: IMPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public MPoint(double x,double y,double z)
        {
            this.X = x; this.Y = y; this.Z = z;
        }
        /// <summary>
        /// 实现IMPoint接口的点类型均可以直接构造转换
        /// </summary>
        /// <param name="point"></param>
        public MPoint(IMPoint point)
        {
            this.X=point.X; this.Y=point.Y; this.Z=point.Z;
        }
    }
    /// <summary>
    /// 点-数学特征：所有三维空间点均具备的属性
    /// </summary>
    public interface IMPoint
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
    }
}
